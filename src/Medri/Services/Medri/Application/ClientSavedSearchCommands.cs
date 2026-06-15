using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    
    
    
    
    
    public static class SavedSearchFormatting
    {
        public static string BuildLabel(SaveSearchRequestDto input)
        {
            var values = new List<string>();
            AddLabels(values, "Contratto", input.Contracts);
            AddLabels(values, "Tipo", input.PropertyTypes);
            AddLabels(values, "Locali", input.Rooms);
            AddLabels(values, "Zona", input.Zones);
            AddLabels(values, "Prezzo", input.PriceRanges);
            AddLabels(values, "Caratteristiche", input.Features);
            AddLabels(values, "Bagni", input.Bathrooms);
            AddLabels(values, "Superficie", input.SurfaceRanges);
            AddLabels(values, "Classe energetica", input.EnergyClasses);
            AddPriceLimit(values, "Prezzo minimo", input.MinPrice);
            AddPriceLimit(values, "Prezzo massimo", input.MaxPrice);
            return values.Count == 0 ? "Tutti gli immobili" : string.Join(" · ", values);
        }

        public static string BuildQueryString(SaveSearchRequestDto input)
        {
            var values = new List<string>();
            AddQueries(values, "contracts", input.Contracts);
            AddQueries(values, "propertyTypes", input.PropertyTypes);
            AddQueries(values, "rooms", input.Rooms);
            AddQueries(values, "zones", input.Zones);
            AddQueries(values, "priceRanges", input.PriceRanges);
            AddQueries(values, "features", input.Features);
            AddQueries(values, "bathrooms", input.Bathrooms);
            AddQueries(values, "surfaceRanges", input.SurfaceRanges);
            AddQueries(values, "energyClasses", input.EnergyClasses);
            AddQuery(values, "minPrice", input.MinPrice?.ToString(System.Globalization.CultureInfo.InvariantCulture));
            AddQuery(values, "maxPrice", input.MaxPrice?.ToString(System.Globalization.CultureInfo.InvariantCulture));
            AddQuery(values, "view", input.View == "split" ? null : input.View);
            AddQuery(values, "sort", input.View == "map" ? null : input.Sort);
            return string.Join("&", values);
        }

        private static void AddPriceLimit(ICollection<string> values, string label, decimal? value)
        {
            if (value.HasValue)
            {
                values.Add(label + ": " + value.Value.ToString("0", System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        private static void AddLabels(ICollection<string> values, string label, IEnumerable<string> items)
        {
            var selectedValues = Normalize(items);
            if (selectedValues.Count > 0)
            {
                values.Add(label + ": " + string.Join(", ", selectedValues));
            }
        }

        private static void AddQueries(ICollection<string> values, string key, IEnumerable<string> items)
        {
            foreach (var value in Normalize(items))
            {
                AddQuery(values, key, value);
            }
        }

        private static void AddQuery(ICollection<string> values, string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                values.Add(Uri.EscapeDataString(key) + "=" + Uri.EscapeDataString(value));
            }
        }

        private static List<string> Normalize(IEnumerable<string> values)
        {
            return (values ?? Array.Empty<string>())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }

    public sealed class SaveClientSearchCommand
    {
        private readonly MedriDbContext dbContext;

        public SaveClientSearchCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task ExecuteAsync(
            Guid userId,
            SaveSearchRequestDto input,
            CancellationToken cancellationToken = default)
        {
            var queryString = SavedSearchFormatting.BuildQueryString(input);

            var alreadySaved = await dbContext.ClientSavedSearches
                .AsNoTracking()
                .AnyAsync(
                    saved => saved.UserId == userId && saved.QueryString == queryString,
                    cancellationToken);
            if (alreadySaved)
            {
                return;
            }

            dbContext.ClientSavedSearches.Add(new ClientSavedSearch
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Label = SavedSearchFormatting.BuildLabel(input),
                QueryString = queryString,
                CreatedAtUtc = DateTime.UtcNow
            });

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public sealed class RemoveClientSavedSearchCommand
    {
        private readonly MedriDbContext dbContext;

        public RemoveClientSavedSearchCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task ExecuteAsync(
            Guid userId,
            Guid savedSearchId,
            CancellationToken cancellationToken = default)
        {
            var saved = await dbContext.ClientSavedSearches
                .FirstOrDefaultAsync(
                    item => item.UserId == userId && item.Id == savedSearchId,
                    cancellationToken);
            if (saved == null)
            {
                return;
            }

            dbContext.ClientSavedSearches.Remove(saved);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
