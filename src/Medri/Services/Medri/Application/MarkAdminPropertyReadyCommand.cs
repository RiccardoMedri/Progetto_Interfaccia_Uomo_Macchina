using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class MarkAdminPropertyReadyCommand
    {
        private readonly MedriDbContext dbContext;

        public MarkAdminPropertyReadyCommand(MedriDbContext dbContext)
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

            var media = await dbContext.PropertyMedia
                .Where(item => item.PropertyListingId == listing.Id)
                .OrderBy(item => item.SortOrder)
                .ToArrayAsync(cancellationToken);
            var completion = AdminPropertyCompletionCalculator.Calculate(
                listing,
                media.Length,
                AdminPropertyCompletionCalculator.HasFloorPlan(media));
            AdminPropertyCompletionCalculator.ApplyToListing(listing, completion);

            if (!completion.IsComplete)
            {
                listing.PublicationStatus = listing.PublicationStatus == PropertyPublicationStatuses.Published
                    ? PropertyPublicationStatuses.NeedsUpdate
                    : PropertyPublicationStatuses.Incomplete;
                listing.FeaturedSortOrder = null;
                listing.UpdatedAtUtc = DateTime.UtcNow;
                await dbContext.SaveChangesAsync(cancellationToken);
                return AdminPropertyCommandResult.Blocked(listing.InternalReference, completion.MissingItems);
            }

            if (listing.PublicationStatus != PropertyPublicationStatuses.Published)
            {
                listing.PublicationStatus = PropertyPublicationStatuses.Ready;
                listing.FeaturedSortOrder = null;
            }
            listing.UpdatedAtUtc = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminPropertyCommandResult.Success(listing.InternalReference);
        }
    }
}
