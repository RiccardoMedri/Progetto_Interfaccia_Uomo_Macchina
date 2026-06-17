using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public class SearchListingsQuery
    {
        private readonly MedriDbContext dbContext;

        public SearchListingsQuery(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<SearchListingsResultDto> ExecuteAsync(
            SearchListingsCriteriaDto criteria,
            Guid? userId,
            CancellationToken cancellationToken = default)
        {
            criteria ??= new SearchListingsCriteriaDto();
            var query = dbContext.PropertyListings.AsNoTracking();
            var contracts = Normalize(criteria.Contracts);
            var propertyTypes = Normalize(criteria.PropertyTypes);
            var rooms = Normalize(criteria.Rooms);
            var zones = Normalize(criteria.Zones);
            var priceRanges = Normalize(criteria.PriceRanges);
            var features = Normalize(criteria.Features);
            var bathrooms = Normalize(criteria.Bathrooms);
            var surfaceRanges = Normalize(criteria.SurfaceRanges);
            var energyClasses = Normalize(criteria.EnergyClasses);
            var minPrice = criteria.MinPrice;
            var maxPrice = criteria.MaxPrice;
            var sort = NormalizeSort(criteria.Sort);

            if (contracts.Count > 0)
            {
                query = query.Where(listing => contracts.Contains(listing.Contract));
            }

            if (propertyTypes.Count > 0)
            {
                query = query.Where(listing => propertyTypes.Contains(listing.PropertyType));
            }

            if (rooms.Count > 0)
            {
                var twoRooms = rooms.Contains("2");
                var threeRooms = rooms.Contains("3");
                var fourOrMoreRooms = rooms.Contains("4-plus");

                query = query.Where(listing =>
                    (twoRooms && listing.Rooms == 2) ||
                    (threeRooms && listing.Rooms == 3) ||
                    (fourOrMoreRooms && listing.Rooms >= 4));
            }

            if (zones.Count > 0)
            {
                query = query.Where(listing => zones.Contains(listing.Zone));
            }

            if (priceRanges.Count > 0)
            {
                var saleUpTo150000 = priceRanges.Contains("sale-up-to-150000");
                var sale150000To250000 = priceRanges.Contains("sale-150000-250000");
                var sale250000To350000 = priceRanges.Contains("sale-250000-350000");
                var sale350000To500000 = priceRanges.Contains("sale-350000-500000");
                var saleOver500000 = priceRanges.Contains("sale-over-500000");
                var rentUpTo500 = priceRanges.Contains("rent-up-to-500");
                var rent500To800 = priceRanges.Contains("rent-500-800");
                var rent800To1200 = priceRanges.Contains("rent-800-1200");
                var rentOver1200 = priceRanges.Contains("rent-over-1200");

                query = query.Where(listing =>
                    (saleUpTo150000 && listing.Contract != "Affitto" && listing.Price <= 150000m) ||
                    (sale150000To250000 && listing.Contract != "Affitto" && listing.Price >= 150000m && listing.Price <= 250000m) ||
                    (sale250000To350000 && listing.Contract != "Affitto" && listing.Price >= 250000m && listing.Price <= 350000m) ||
                    (sale350000To500000 && listing.Contract != "Affitto" && listing.Price >= 350000m && listing.Price <= 500000m) ||
                    (saleOver500000 && listing.Contract != "Affitto" && listing.Price > 500000m) ||
                    (rentUpTo500 && listing.Contract == "Affitto" && listing.Price <= 500m) ||
                    (rent500To800 && listing.Contract == "Affitto" && listing.Price >= 500m && listing.Price <= 800m) ||
                    (rent800To1200 && listing.Contract == "Affitto" && listing.Price >= 800m && listing.Price <= 1200m) ||
                    (rentOver1200 && listing.Contract == "Affitto" && listing.Price > 1200m));
            }

            if (features.Count > 0)
            {
                var garden = features.Contains("garden");
                var terrace = features.Contains("terrace");
                var garage = features.Contains("garage");
                var parking = features.Contains("parking");
                var elevator = features.Contains("elevator");
                var furnished = features.Contains("furnished");
                var moveInReady = features.Contains("move-in-ready");
                var renovationNeeded = features.Contains("renovation-needed");
                var highEnergyClass = features.Contains("high-energy-class");
                var nearServices = features.Contains("near-services");
                var nearPublicTransport = features.Contains("near-public-transport");

                query = query.Where(listing =>
                    (garden && listing.FeatureKeys.Contains("|garden|")) ||
                    (terrace && listing.FeatureKeys.Contains("|terrace|")) ||
                    (garage && listing.FeatureKeys.Contains("|garage|")) ||
                    (parking && listing.FeatureKeys.Contains("|parking|")) ||
                    (elevator && listing.FeatureKeys.Contains("|elevator|")) ||
                    (furnished && listing.FeatureKeys.Contains("|furnished|")) ||
                    (moveInReady && listing.FeatureKeys.Contains("|move-in-ready|")) ||
                    (renovationNeeded && listing.FeatureKeys.Contains("|renovation-needed|")) ||
                    (highEnergyClass && listing.FeatureKeys.Contains("|high-energy-class|")) ||
                    (nearServices && listing.FeatureKeys.Contains("|near-services|")) ||
                    (nearPublicTransport && listing.FeatureKeys.Contains("|near-public-transport|")));
            }

            if (bathrooms.Count > 0)
            {
                var oneBath = bathrooms.Contains("1");
                var twoBaths = bathrooms.Contains("2");
                var threeOrMoreBaths = bathrooms.Contains("3-plus");

                query = query.Where(listing =>
                    (oneBath && listing.Bathrooms == 1) ||
                    (twoBaths && listing.Bathrooms == 2) ||
                    (threeOrMoreBaths && listing.Bathrooms >= 3));
            }

            if (surfaceRanges.Count > 0)
            {
                var upTo60 = surfaceRanges.Contains("up-to-60");
                var from60To100 = surfaceRanges.Contains("60-100");
                var from100To150 = surfaceRanges.Contains("100-150");
                var over150 = surfaceRanges.Contains("over-150");

                query = query.Where(listing =>
                    (upTo60 && listing.SurfaceSquareMeters <= 60) ||
                    (from60To100 && listing.SurfaceSquareMeters > 60 && listing.SurfaceSquareMeters <= 100) ||
                    (from100To150 && listing.SurfaceSquareMeters > 100 && listing.SurfaceSquareMeters <= 150) ||
                    (over150 && listing.SurfaceSquareMeters > 150));
            }

            if (energyClasses.Count > 0)
            {
                query = query.Where(listing => energyClasses.Contains(listing.EnergyClass));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(listing => listing.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(listing => listing.Price <= maxPrice.Value);
            }

            if (criteria.FocusListingId.HasValue)
            {
                query = query.Where(listing => listing.Id == criteria.FocusListingId.Value);
            }

            var orderedQuery = sort switch
            {
                "price-asc" => query.OrderBy(listing => listing.Price).ThenBy(listing => listing.SortOrder),
                "price-desc" => query.OrderByDescending(listing => listing.Price).ThenBy(listing => listing.SortOrder),
                "size-asc" => query.OrderBy(listing => listing.SurfaceSquareMeters).ThenBy(listing => listing.SortOrder),
                "size-desc" => query.OrderByDescending(listing => listing.SurfaceSquareMeters).ThenBy(listing => listing.SortOrder),
                _ => query.OrderBy(listing => listing.SortOrder)
            };

            var listings = criteria.IncludeListings
                ? await orderedQuery
                    .Select(listing => new PropertySearchCardDto
                    {
                        Id = listing.Id,
                        Title = listing.Title,
                        Slug = listing.Slug,
                        Status = listing.Status,
                        Price = listing.Price,
                        Contract = listing.Contract,
                        DisplayLocation = listing.DisplayLocation,
                        Location = listing.Location,
                        Rooms = listing.Rooms,
                        Bathrooms = listing.Bathrooms,
                        SurfaceSquareMeters = listing.SurfaceSquareMeters,
                        ImageUrl = listing.ImageUrl,
                        SortOrder = listing.SortOrder,
                        FeaturedSortOrder = listing.FeaturedSortOrder,
                        Latitude = listing.Latitude,
                        Longitude = listing.Longitude,
                        IsSaved = userId.HasValue &&
                            dbContext.FavoriteProperties.Any(
                                favorite => favorite.UserId == userId.Value &&
                                    favorite.PropertyListingId == listing.Id)
                    })
                    .ToListAsync(cancellationToken)
                : new List<PropertySearchCardDto>();

            var mapListings = criteria.IncludeMapListings
                ? await query
                    .OrderBy(listing => listing.SortOrder)
                    .Select(listing => new PropertySearchCardDto
                    {
                        Id = listing.Id,
                        Title = listing.Title,
                        Slug = listing.Slug,
                        Status = listing.Status,
                        Price = listing.Price,
                        Contract = listing.Contract,
                        DisplayLocation = listing.DisplayLocation,
                        Location = listing.Location,
                        Rooms = listing.Rooms,
                        Bathrooms = listing.Bathrooms,
                        SurfaceSquareMeters = listing.SurfaceSquareMeters,
                        ImageUrl = listing.ImageUrl,
                        SortOrder = listing.SortOrder,
                        FeaturedSortOrder = listing.FeaturedSortOrder,
                        Latitude = listing.Latitude,
                        Longitude = listing.Longitude,
                        IsSaved = userId.HasValue &&
                            dbContext.FavoriteProperties.Any(
                                favorite => favorite.UserId == userId.Value &&
                                    favorite.PropertyListingId == listing.Id)
                    })
                    .ToListAsync(cancellationToken)
                : new List<PropertySearchCardDto>();

            return new SearchListingsResultDto
            {
                Listings = listings,
                MapListings = mapListings
            };
        }

        private static List<string> Normalize(IEnumerable<string> values)
        {
            return (values ?? Array.Empty<string>())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string NormalizeSort(string value)
        {
            var normalized = value?.Trim().ToLowerInvariant();
            return normalized == "price-asc" ||
                normalized == "price-desc" ||
                normalized == "size-asc" ||
                normalized == "size-desc"
                    ? normalized
                    : null;
        }
    }

    public class HomeFeaturedListingsQuery
    {
        private readonly MedriDbContext dbContext;

        public HomeFeaturedListingsQuery(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IReadOnlyList<PropertySearchCardDto>> ExecuteAsync(
            Guid? userId,
            CancellationToken cancellationToken = default)
        {
            return await dbContext.PropertyListings
                .AsNoTracking()
                .Where(listing => listing.FeaturedSortOrder.HasValue)
                .OrderBy(listing => listing.FeaturedSortOrder.Value)
                .ThenBy(listing => listing.SortOrder)
                .Take(3)
                .Select(listing => new PropertySearchCardDto
                {
                    Id = listing.Id,
                    Title = listing.Title,
                    Slug = listing.Slug,
                    Status = listing.Status,
                    Price = listing.Price,
                    Contract = listing.Contract,
                    DisplayLocation = listing.DisplayLocation,
                    Location = listing.Location,
                    Rooms = listing.Rooms,
                    Bathrooms = listing.Bathrooms,
                    SurfaceSquareMeters = listing.SurfaceSquareMeters,
                    ImageUrl = listing.ImageUrl,
                    SortOrder = listing.SortOrder,
                    FeaturedSortOrder = listing.FeaturedSortOrder,
                    IsSaved = userId.HasValue &&
                        dbContext.FavoriteProperties.Any(
                            favorite => favorite.UserId == userId.Value &&
                                favorite.PropertyListingId == listing.Id)
                })
                .ToListAsync(cancellationToken);
        }
    }

    public class PropertyDetailQuery
    {
        private readonly MedriDbContext dbContext;

        public PropertyDetailQuery(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<PropertyDetailDto> ExecuteAsync(
            string slug,
            Guid? userId,
            bool includeUnpublished,
            CancellationToken cancellationToken = default)
        {
            var listings = dbContext.PropertyListings.AsNoTracking();
            if (includeUnpublished)
            {
                // Admin "Anteprima" embeds this public page in an iframe to preview a listing
                // before publication: bypass the published-only global query filter in that case.
                listings = listings.IgnoreQueryFilters();
            }

            var detail = await listings
                .Where(property => property.Slug == slug)
                .Select(property => new PropertyDetailDto
                {
                    Id = property.Id,
                    InternalReference = property.InternalReference,
                    Title = property.Title,
                    Slug = property.Slug,
                    Status = property.Status,
                    Price = property.Price,
                    Contract = property.Contract,
                    IsSaved = userId.HasValue &&
                        dbContext.FavoriteProperties.Any(
                            favorite => favorite.UserId == userId.Value &&
                                favorite.PropertyListingId == property.Id),
                    Address = property.Address,
                    AssignedAgencyUserId = property.AssignedAgencyUserId,
                    PropertyType = property.PropertyType,
                    Zone = property.Zone,
                    DisplayLocation = property.DisplayLocation,
                    Location = property.Location,
                    ImageUrl = property.ImageUrl,
                    Rooms = property.Rooms,
                    Bathrooms = property.Bathrooms,
                    SurfaceSquareMeters = property.SurfaceSquareMeters,
                    BedroomsLabel = property.BedroomsLabel,
                    FloorLabel = property.FloorLabel,
                    GarageLabel = property.GarageLabel,
                    OutdoorSpaceLabel = property.OutdoorSpaceLabel,
                    EnergyClass = property.EnergyClass,
                    AvailabilityLabel = property.AvailabilityLabel,
                    HeatingLabel = property.HeatingLabel,
                    RequiredWorksLabel = property.RequiredWorksLabel,
                    ConstructionYearLabel = property.ConstructionYearLabel,
                    CondoFeesLabel = property.CondoFeesLabel,
                    BalconyLabel = property.BalconyLabel,
                    CellarLabel = property.CellarLabel,
                    NearbyServicesLabel = property.NearbyServicesLabel,
                    SummaryTitle = property.SummaryTitle,
                    SummaryParagraph1 = property.SummaryParagraph1,
                    SummaryParagraph2 = property.SummaryParagraph2,
                    ReadinessNote = property.ReadinessNote,
                    CostsNote = property.CostsNote,
                    ContextNote = property.ContextNote,
                    DecisionMarginNote = property.DecisionMarginNote,
                    HumanFitNote = property.HumanFitNote
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (detail == null)
            {
                return null;
            }

            if (detail.AssignedAgencyUserId.HasValue)
            {
                var advisor = await dbContext.AgencyUsers
                    .AsNoTracking()
                    .Where(item => item.Id == detail.AssignedAgencyUserId.Value)
                    .Select(item => new
                    {
                        item.DisplayName,
                        item.Role
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                detail.AdvisorDisplayName = advisor?.DisplayName;
                detail.AdvisorRole = advisor?.Role;
            }

            detail.Media = await dbContext.PropertyMedia
                .AsNoTracking()
                .Where(media => media.PropertyListingId == detail.Id)
                .OrderBy(media => media.SortOrder)
                .Select(media => new PropertyMediaDto
                {
                    Url = media.Url,
                    AltText = media.AltText,
                    SortOrder = media.SortOrder
                })
                .ToListAsync(cancellationToken);

            detail.SimilarProperties = await dbContext.PropertyListings
                .AsNoTracking()
                .Where(property => property.Slug != slug)
                .OrderByDescending(property => property.Contract == detail.Contract)
                .ThenByDescending(property => property.PropertyType == detail.PropertyType)
                .ThenByDescending(property => property.Zone == detail.Zone)
                .ThenBy(property => Math.Abs(property.Price - detail.Price))
                .ThenBy(property => Math.Abs(property.SurfaceSquareMeters - detail.SurfaceSquareMeters))
                .ThenBy(property => Math.Abs(property.Rooms - detail.Rooms))
                .ThenBy(property => property.SortOrder)
                .Take(3)
                .Select(property => new PropertySummaryDto
                {
                    Id = property.Id,
                    Title = property.Title,
                    Slug = property.Slug,
                    Status = property.Status,
                    Price = property.Price,
                    Contract = property.Contract,
                    DisplayLocation = property.DisplayLocation,
                    Location = property.Location,
                    ImageUrl = property.ImageUrl
                })
                .ToListAsync(cancellationToken);

            return detail;
        }
    }

    public class GetFavoritePropertiesQuery
    {
        private readonly MedriDbContext dbContext;

        public GetFavoritePropertiesQuery(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IReadOnlyList<FavoritePropertyDto>> ExecuteAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await dbContext.FavoriteProperties
                .AsNoTracking()
                .Where(favorite => favorite.UserId == userId)
                .Join(
                    dbContext.PropertyListings.AsNoTracking(),
                    favorite => favorite.PropertyListingId,
                    property => property.Id,
                    (favorite, property) => new { favorite, property })
                .OrderBy(item => item.property.SortOrder)
                .Select(item => new FavoritePropertyDto
                {
                    Id = item.property.Id,
                    Title = item.property.Title,
                    Slug = item.property.Slug,
                    Status = item.property.Status,
                    Price = item.property.Price,
                    Contract = item.property.Contract,
                    DisplayLocation = item.property.DisplayLocation,
                    Location = item.property.Location,
                    ImageUrl = item.property.ImageUrl,
                    Rooms = item.property.Rooms,
                    Bathrooms = item.property.Bathrooms,
                    SurfaceSquareMeters = item.property.SurfaceSquareMeters,
                    BedroomsLabel = item.property.BedroomsLabel,
                    GarageLabel = item.property.GarageLabel,
                    OutdoorSpaceLabel = item.property.OutdoorSpaceLabel,
                    EnergyClass = item.property.EnergyClass,
                    IsSaved = true
                })
                .ToListAsync(cancellationToken);
        }

        internal static List<Guid> ParseIds(string ids, int take)
        {
            return (ids ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(value => Guid.TryParse(value, out var id) ? id : Guid.Empty)
                .Where(id => id != Guid.Empty)
                .Distinct()
                .Take(take)
                .ToList();
        }
    }

    public class FavoritePropertyCountQuery
    {
        private readonly MedriDbContext dbContext;

        public FavoritePropertyCountQuery(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<int> ExecuteAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return dbContext.FavoriteProperties
                .AsNoTracking()
                .CountAsync(favorite => favorite.UserId == userId, cancellationToken);
        }
    }

    public class ComparisonQuery
    {
        private readonly MedriDbContext dbContext;

        public ComparisonQuery(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IReadOnlyList<ComparisonPropertyDto>> ExecuteAsync(
            string ids,
            CancellationToken cancellationToken = default)
        {
            var selectedIds = GetFavoritePropertiesQuery.ParseIds(ids, 4);
            if (selectedIds.Count == 0)
            {
                return new List<ComparisonPropertyDto>();
            }

            var query = dbContext.PropertyListings
                .AsNoTracking()
                .Where(property => selectedIds.Contains(property.Id));

            var listings = await query
                .OrderBy(property => property.SortOrder)
                .Take(4)
                .Select(property => new ComparisonPropertyDto
                {
                    Id = property.Id,
                    Title = property.Title,
                    Slug = property.Slug,
                    Status = property.Status,
                    Price = property.Price,
                    Contract = property.Contract,
                    DisplayLocation = property.DisplayLocation,
                    Location = property.Location,
                    ImageUrl = property.ImageUrl,
                    ZoneComparisonLabel = property.ZoneComparisonLabel,
                    SurfaceRoomsComparisonLabel = property.SurfaceRoomsComparisonLabel,
                    StatusWorksComparisonLabel = property.StatusWorksComparisonLabel,
                    OutdoorSpaceLabel = property.OutdoorSpaceLabel,
                    EnergyClass = property.EnergyClass,
                    MainCompromise = property.MainCompromise,
                    BedroomsLabel = property.BedroomsLabel,
                    Bathrooms = property.Bathrooms,
                    AccessLabel = property.AccessLabel,
                    GarageLabel = property.GarageLabel,
                    HeatingLabel = property.HeatingLabel,
                    ConstructionYearLabel = property.ConstructionYearLabel,
                    ManagementCostsLabel = property.ManagementCostsLabel,
                    EstimatedWorksLabel = property.EstimatedWorksLabel,
                    EnergyCostsLabel = property.EnergyCostsLabel,
                    PersonalizationLabel = property.PersonalizationLabel,
                    AvailabilityLabel = property.AvailabilityLabel,
                    NearbyServicesLabel = property.NearbyServicesLabel,
                    TransportLabel = property.TransportLabel,
                    PrivacyLabel = property.PrivacyLabel,
                    NoiseLabel = property.NoiseLabel,
                    IdealTargetLabel = property.IdealTargetLabel
                })
                .ToListAsync(cancellationToken);

            return selectedIds.Count == 0
                ? listings
                : listings.OrderBy(property => selectedIds.IndexOf(property.Id)).ToList();
        }
    }
}
