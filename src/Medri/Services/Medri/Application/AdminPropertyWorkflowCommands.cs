using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class PublishAdminPropertyCommand
    {
        private readonly MedriDbContext dbContext;

        public PublishAdminPropertyCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminPropertyCommandResult> ExecuteAsync(
            string reference,
            CancellationToken cancellationToken = default)
        {
            var listing = await FindListingAsync(reference, cancellationToken);
            if (listing == null)
            {
                return AdminPropertyCommandResult.NotFound();
            }

            var media = await MediaAsync(listing.Id, cancellationToken);
            var completion = AdminPropertyCompletionCalculator.Calculate(
                listing,
                media.Length);
            AdminPropertyCompletionCalculator.ApplyToListing(listing, completion);
            listing.UpdatedAtUtc = DateTime.UtcNow;

            if (!completion.IsComplete)
            {
                listing.PublicationStatus = listing.PublicationStatus == PropertyPublicationStatuses.Published
                    ? PropertyPublicationStatuses.NeedsUpdate
                    : PropertyPublicationStatuses.Incomplete;
                listing.FeaturedSortOrder = null;
                await dbContext.SaveChangesAsync(cancellationToken);
                return AdminPropertyCommandResult.Blocked(listing.InternalReference, completion.MissingItems);
            }

            listing.PublicationStatus = PropertyPublicationStatuses.Published;
            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminPropertyCommandResult.Success(listing.InternalReference);
        }

        private Task<PropertyListing> FindListingAsync(
            string reference,
            CancellationToken cancellationToken)
        {
            return string.IsNullOrWhiteSpace(reference)
                ? Task.FromResult<PropertyListing>(null)
                : dbContext.PropertyListings
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(
                        item => item.InternalReference == reference,
                        cancellationToken);
        }

        private Task<PropertyMedia[]> MediaAsync(
            Guid listingId,
            CancellationToken cancellationToken)
        {
            return dbContext.PropertyMedia
                .Where(media => media.PropertyListingId == listingId)
                .OrderBy(media => media.SortOrder)
                .ToArrayAsync(cancellationToken);
        }
    }

    public sealed class ArchiveAdminPropertyCommand
    {
        private readonly MedriDbContext dbContext;

        public ArchiveAdminPropertyCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminPropertyCommandResult> ExecuteAsync(
            string reference,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return AdminPropertyCommandResult.NotFound();
            }

            var listing = await dbContext.PropertyListings
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    item => item.InternalReference == reference,
                    cancellationToken);

            if (listing == null)
            {
                return AdminPropertyCommandResult.NotFound();
            }

            listing.PublicationStatus = PropertyPublicationStatuses.Archived;
            listing.FeaturedSortOrder = null;
            listing.UpdatedAtUtc = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);
            await FeaturedPropertySlots.NormalizeAsync(dbContext, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminPropertyCommandResult.Success(listing.InternalReference);
        }
    }

    public sealed class DiscardDraftAdminPropertyCommand
    {
        private readonly MedriDbContext dbContext;

        public DiscardDraftAdminPropertyCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminPropertyCommandResult> ExecuteAsync(
            string reference,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return AdminPropertyCommandResult.NotFound();
            }

            var listing = await dbContext.PropertyListings
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    item => item.InternalReference == reference,
                    cancellationToken);

            if (listing == null)
            {
                return AdminPropertyCommandResult.NotFound();
            }

            if (listing.PublicationStatus != PropertyPublicationStatuses.Incomplete)
            {
                return AdminPropertyCommandResult.Success(listing.InternalReference);
            }

            var media = await dbContext.PropertyMedia
                .Where(item => item.PropertyListingId == listing.Id)
                .ToArrayAsync(cancellationToken);
            dbContext.PropertyMedia.RemoveRange(media);
            dbContext.PropertyListings.Remove(listing);
            await dbContext.SaveChangesAsync(cancellationToken);

            return AdminPropertyCommandResult.Success(reference);
        }
    }

    public sealed class BulkAssignAdminPropertiesCommand
    {
        private readonly MedriDbContext dbContext;

        public BulkAssignAdminPropertiesCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminPropertyBulkCommandResult> ExecuteAsync(
            IEnumerable<string> references,
            Guid? assignedAgencyUserId,
            CancellationToken cancellationToken = default)
        {
            var selectedReferences = ApplicationText.CleanReferences(references);
            if (selectedReferences.Count == 0 || !assignedAgencyUserId.HasValue)
            {
                return new AdminPropertyBulkCommandResult();
            }

            var advisorExists = await StaffUserQueries.Assignable(dbContext)
                .AnyAsync(
                    advisor => advisor.Id == assignedAgencyUserId.Value,
                    cancellationToken);

            if (!advisorExists)
            {
                return new AdminPropertyBulkCommandResult();
            }

            var listings = await dbContext.PropertyListings
                .IgnoreQueryFilters()
                .Where(listing =>
                    listing.InternalReference != null &&
                    selectedReferences.Contains(listing.InternalReference) &&
                    listing.PublicationStatus != PropertyPublicationStatuses.Archived &&
                    listing.AssignedAgencyUserId != assignedAgencyUserId.Value)
                .ToArrayAsync(cancellationToken);

            foreach (var listing in listings)
            {
                listing.AssignedAgencyUserId = assignedAgencyUserId;
                listing.UpdatedAtUtc = DateTime.UtcNow;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            return new AdminPropertyBulkCommandResult
            {
                UpdatedCount = listings.Length
            };
        }

    }

    public sealed class FeatureAdminPropertyCommand
    {
        private readonly MedriDbContext dbContext;

        public FeatureAdminPropertyCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminPropertyCommandResult> ExecuteAsync(
            string reference,
            CancellationToken cancellationToken = default)
        {
            var listing = await FindListingAsync(reference, cancellationToken);
            if (listing == null)
            {
                return AdminPropertyCommandResult.NotFound();
            }

            if (listing.PublicationStatus == PropertyPublicationStatuses.Archived)
            {
                return AdminPropertyCommandResult.Blocked(listing.InternalReference, "Immobile archiviato");
            }

            if (listing.PublicationStatus != PropertyPublicationStatuses.Published)
            {
                return AdminPropertyCommandResult.Blocked(
                    listing.InternalReference,
                    "pubblica l'immobile prima di inserirlo in homepage");
            }

            if (listing.FeaturedSortOrder.HasValue)
            {
                return AdminPropertyCommandResult.Success(listing.InternalReference);
            }

            var featured = await FeaturedListingsAsync(cancellationToken);
            if (featured.Length >= FeaturedPropertySlots.MaxSlots)
            {
                featured[FeaturedPropertySlots.MaxSlots - 1].FeaturedSortOrder = null;
                featured = featured.Take(FeaturedPropertySlots.MaxSlots - 1).ToArray();
            }

            listing.FeaturedSortOrder = featured.Length + 1;
            listing.UpdatedAtUtc = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminPropertyCommandResult.Success(listing.InternalReference);
        }

        private Task<PropertyListing> FindListingAsync(
            string reference,
            CancellationToken cancellationToken)
        {
            return string.IsNullOrWhiteSpace(reference)
                ? Task.FromResult<PropertyListing>(null)
                : dbContext.PropertyListings
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(
                        item => item.InternalReference == reference,
                        cancellationToken);
        }

        private Task<PropertyListing[]> FeaturedListingsAsync(CancellationToken cancellationToken)
        {
            return FeaturedPropertySlots.ActiveAsync(dbContext, cancellationToken);
        }
    }

    public sealed class MoveFeaturedAdminPropertyCommand
    {
        private readonly MedriDbContext dbContext;

        public MoveFeaturedAdminPropertyCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminPropertyCommandResult> ExecuteAsync(
            string reference,
            int direction,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference) || direction == 0)
            {
                return AdminPropertyCommandResult.NotFound();
            }

            var featured = await dbContext.PropertyListings
                .IgnoreQueryFilters()
                .Where(listing => listing.FeaturedSortOrder.HasValue &&
                                  listing.PublicationStatus == PropertyPublicationStatuses.Published)
                .OrderBy(listing => listing.FeaturedSortOrder.Value)
                .ThenBy(listing => listing.SortOrder)
                .ToArrayAsync(cancellationToken);
            var index = Array.FindIndex(
                featured,
                listing => string.Equals(listing.InternalReference, reference, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                return AdminPropertyCommandResult.NotFound();
            }

            var targetIndex = index + Math.Sign(direction);
            if (targetIndex < 0 || targetIndex >= featured.Length)
            {
                return AdminPropertyCommandResult.Success(featured[index].InternalReference);
            }

            (featured[index].FeaturedSortOrder, featured[targetIndex].FeaturedSortOrder) =
                (featured[targetIndex].FeaturedSortOrder, featured[index].FeaturedSortOrder);
            featured[index].UpdatedAtUtc = DateTime.UtcNow;
            featured[targetIndex].UpdatedAtUtc = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminPropertyCommandResult.Success(reference);
        }
    }

    public sealed class RemoveFeaturedAdminPropertyCommand
    {
        private readonly MedriDbContext dbContext;

        public RemoveFeaturedAdminPropertyCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminPropertyCommandResult> ExecuteAsync(
            string reference,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return AdminPropertyCommandResult.NotFound();
            }

            var listing = await dbContext.PropertyListings
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    item => item.InternalReference == reference,
                    cancellationToken);
            if (listing == null)
            {
                return AdminPropertyCommandResult.NotFound();
            }

            listing.FeaturedSortOrder = null;
            listing.UpdatedAtUtc = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);

            await FeaturedPropertySlots.NormalizeAsync(dbContext, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminPropertyCommandResult.Success(listing.InternalReference);
        }
    }

    public sealed class AddAdminPropertyMediaCommand
    {
        private readonly MedriDbContext dbContext;

        public AddAdminPropertyMediaCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminPropertyCommandResult> ExecuteAsync(
            string reference,
            string url,
            string altText,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference) || string.IsNullOrWhiteSpace(url))
            {
                return AdminPropertyCommandResult.NotFound();
            }

            var listing = await dbContext.PropertyListings
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    item => item.InternalReference == reference,
                    cancellationToken);
            if (listing == null)
            {
                return AdminPropertyCommandResult.NotFound();
            }

            var media = await dbContext.PropertyMedia
                .Where(item => item.PropertyListingId == listing.Id)
                .OrderBy(item => item.SortOrder)
                .ToArrayAsync(cancellationToken);
            var nextSortOrder = media
                .Select(item => item.SortOrder)
                .DefaultIfEmpty(0)
                .Max() + 1;

            var newMedia = new PropertyMedia
            {
                Id = Guid.NewGuid(),
                PropertyListingId = listing.Id,
                Url = url.Trim(),
                AltText = string.IsNullOrWhiteSpace(altText) ? listing.Title : altText.Trim(),
                SortOrder = nextSortOrder
            };
            dbContext.PropertyMedia.Add(newMedia);

            var activeMedia = media.Concat(new[] { newMedia }).ToArray();
            if (activeMedia.Length == 1)
            {
                listing.ImageUrl = newMedia.Url;
            }

            var completion = AdminPropertyCompletionCalculator.Calculate(
                listing,
                activeMedia.Length);
            AdminPropertyCompletionCalculator.ApplyToListing(listing, completion);
            if (listing.PublicationStatus != PropertyPublicationStatuses.Archived)
            {
                listing.PublicationStatus = completion.IsComplete
                    ? ReadyOrPublished(listing.PublicationStatus)
                    : IncompleteOrNeedsUpdate(listing.PublicationStatus);
            }

            listing.UpdatedAtUtc = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminPropertyCommandResult.Success(listing.InternalReference);
        }

        private static string ReadyOrPublished(string status)
        {
            return status == PropertyPublicationStatuses.Published
                ? PropertyPublicationStatuses.Published
                : PropertyPublicationStatuses.Ready;
        }

        private static string IncompleteOrNeedsUpdate(string status)
        {
            return status == PropertyPublicationStatuses.Published
                ? PropertyPublicationStatuses.NeedsUpdate
                : PropertyPublicationStatuses.Incomplete;
        }
    }
}
