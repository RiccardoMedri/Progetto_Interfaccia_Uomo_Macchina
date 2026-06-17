using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class UpdateAdminPropertyDetailCommand
    {
        private readonly MedriDbContext dbContext;

        public UpdateAdminPropertyDetailCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminPropertyCommandResult> ExecuteAsync(
            string reference,
            AdminPropertyDetailUpdateDto update,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference) || update == null)
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

            listing.Title = update.Title;
            listing.ListingCategory = update.ListingCategory;
            listing.Contract = update.Contract;
            listing.Status = update.Contract;
            listing.Price = update.Price;
            listing.DisplayLocation = update.DisplayLocation;
            listing.Location = update.DisplayLocation;
            listing.Address = update.Address;
            // Keep the existing coordinates if geocoding was unavailable / produced nothing,
            // so editing an address while the map service is down never resets the pin to (0,0).
            if (update.Latitude.HasValue && update.Longitude.HasValue)
            {
                listing.Latitude = update.Latitude.Value;
                listing.Longitude = update.Longitude.Value;
            }
            listing.SurfaceSquareMeters = update.SurfaceSquareMeters;
            listing.Rooms = update.Rooms;
            listing.BedroomsLabel = update.BedroomsLabel;
            listing.Bathrooms = update.Bathrooms;
            listing.EnergyClass = update.EnergyClass;
            listing.RequiredWorksLabel = update.RequiredWorksLabel;
            listing.CondoFeesLabel = update.CondoFeesLabel;
            listing.FloorLabel = update.FloorLabel;
            listing.ElevatorLabel = update.ElevatorLabel;
            listing.OutdoorSpaceLabel = update.OutdoorSpaceLabel;
            listing.GarageLabel = update.GarageLabel;
            listing.SummaryTitle = ApplicationText.Clean(update.SummaryTitle);
            listing.SummaryParagraph1 = update.Description;
            listing.SummaryParagraph2 = ApplicationText.Clean(update.SummaryParagraph2);
            listing.ReadinessNote = ApplicationText.Clean(update.ReadinessNote);
            listing.CostsNote = ApplicationText.Clean(update.CostsNote);
            listing.ContextNote = ApplicationText.Clean(update.ContextNote);
            listing.DecisionMarginNote = ApplicationText.Clean(update.DecisionMarginNote);
            listing.AvailabilityLabel = ApplicationText.Clean(update.AvailabilityLabel);
            listing.HeatingLabel = ApplicationText.Clean(update.HeatingLabel);
            listing.ConstructionYearLabel = ApplicationText.Clean(update.ConstructionYearLabel);
            listing.BalconyLabel = ApplicationText.Clean(update.BalconyLabel);
            listing.CellarLabel = ApplicationText.Clean(update.CellarLabel);
            listing.NearbyServicesLabel = ApplicationText.Clean(update.NearbyServicesLabel);
            listing.HumanFitNote = update.HumanFitNote;
            listing.AssignedAgencyUserId = update.AssignedAgencyUserId;
            listing.UpdatedAtUtc = DateTime.UtcNow;

            var mediaUpdates = update.Media ?? Array.Empty<AdminPropertyMediaUpdateDto>();
            var fallbackMediaInput = mediaUpdates
                .FirstOrDefault(media => media.Id == Guid.Empty && !media.Remove);
            var mediaInputs = mediaUpdates
                .Where(media => media.Id != Guid.Empty)
                .GroupBy(media => media.Id)
                .Select(group => group.First())
                .ToDictionary(media => media.Id);
            var mediaRows = await dbContext.PropertyMedia
                .Where(media => media.PropertyListingId == listing.Id)
                .OrderBy(media => media.SortOrder)
                .ToArrayAsync(cancellationToken);

            if (mediaRows.Length == 0 &&
                fallbackMediaInput != null &&
                ShouldPromoteListingImageAsMedia(listing))
            {
                var fallbackMedia = new PropertyMedia
                {
                    Id = Guid.NewGuid(),
                    PropertyListingId = listing.Id,
                    Url = listing.ImageUrl.Trim(),
                    AltText = ApplicationText.Clean(fallbackMediaInput.AltText) ?? listing.Title,
                    SortOrder = 1
                };
                dbContext.PropertyMedia.Add(fallbackMedia);
                mediaRows = new[] { fallbackMedia };
            }

            if (mediaInputs.Count > 0)
            {
                foreach (var media in mediaRows)
                {
                    if (!mediaInputs.TryGetValue(media.Id, out var input))
                    {
                        continue;
                    }

                    if (input.Remove)
                    {
                        dbContext.PropertyMedia.Remove(media);
                        continue;
                    }

                    media.AltText = ApplicationText.Clean(input.AltText);
                    media.SortOrder = input.SortOrder <= 0
                        ? media.SortOrder
                        : input.SortOrder;
                }
            }

            var activeMedia = mediaRows
                .Where(media => !mediaInputs.TryGetValue(media.Id, out var input) || !input.Remove)
                .OrderBy(media => media.SortOrder)
                .ThenBy(media => media.Id)
                .ToArray();

            for (var index = 0; index < activeMedia.Length; index++)
            {
                activeMedia[index].SortOrder = index + 1;
            }

            listing.ImageUrl = activeMedia.Length == 0
                ? string.Empty
                : activeMedia[0].Url;

            var completion = AdminPropertyCompletionCalculator.Calculate(
                listing,
                activeMedia.Length);
            AdminPropertyCompletionCalculator.ApplyToListing(listing, completion);
            UpdatePublicationStatusAfterSave(listing, completion);

            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminPropertyCommandResult.Success(listing.InternalReference);
        }

        private static bool ShouldPromoteListingImageAsMedia(PropertyListing listing)
        {
            return listing != null &&
                !string.IsNullOrWhiteSpace(listing.ImageUrl) &&
                (listing.PublicationStatus != PropertyPublicationStatuses.Incomplete ||
                 listing.ImageUrl.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase));
        }

        private static void UpdatePublicationStatusAfterSave(
            PropertyListing listing,
            AdminPropertyCompletionResult completion)
        {
            if (listing.PublicationStatus == PropertyPublicationStatuses.Archived)
            {
                return;
            }

            if (!completion.IsComplete)
            {
                listing.PublicationStatus = listing.PublicationStatus == PropertyPublicationStatuses.Published
                    ? PropertyPublicationStatuses.NeedsUpdate
                    : PropertyPublicationStatuses.Incomplete;
                listing.FeaturedSortOrder = null;
                return;
            }

            if (listing.PublicationStatus == PropertyPublicationStatuses.Incomplete ||
                listing.PublicationStatus == PropertyPublicationStatuses.NeedsUpdate)
            {
                listing.PublicationStatus = PropertyPublicationStatuses.Ready;
                listing.FeaturedSortOrder = null;
            }
        }

    }
}
