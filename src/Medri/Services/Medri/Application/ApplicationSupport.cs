using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    internal static class ApplicationText
    {
        public static string Clean(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        public static string FullName(
            string firstName,
            string lastName,
            string fallback)
        {
            var parts = new[] { Clean(firstName), Clean(lastName) }
                .Where(part => !string.IsNullOrWhiteSpace(part));

            var fullName = string.Join(" ", parts);
            return string.IsNullOrWhiteSpace(fullName) ? fallback : fullName;
        }

        public static IReadOnlyList<string> CleanReferences(IEnumerable<string> references)
        {
            return (references ?? Array.Empty<string>())
                .Where(reference => !string.IsNullOrWhiteSpace(reference))
                .Select(reference => reference.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public static string NormalizeTags(string value)
        {
            var tags = (value ?? string.Empty)
                .Split(new[] { "\r\n", "\n", "|", "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return tags.Length == 0 ? null : string.Join("|", tags);
        }

        public static int NormalizePage(int requestedPage, int totalItems, int pageSize)
        {
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
            return Math.Min(Math.Max(1, requestedPage), totalPages);
        }

        public static int NormalizePageSize(
            int requestedPageSize,
            int defaultPageSize,
            params int[] allowedPageSizes)
        {
            return allowedPageSizes.Contains(requestedPageSize)
                ? requestedPageSize
                : defaultPageSize;
        }
    }

    internal static class AdminReferenceGenerator
    {
        public static Task<string> NextLeadReferenceAsync(
            MedriDbContext dbContext,
            CancellationToken cancellationToken)
        {
            return NextAsync(
                dbContext.Leads
                    .AsNoTracking()
                    .Where(lead => lead.InternalReference != null &&
                                   lead.InternalReference.StartsWith("LD-"))
                    .Select(lead => lead.InternalReference),
                "LD-",
                defaultNumber: 0,
                cancellationToken);
        }

        public static Task<string> NextRequestReferenceAsync(
            MedriDbContext dbContext,
            CancellationToken cancellationToken,
            int defaultNumber = 0)
        {
            return NextAsync(
                dbContext.SearchProfiles
                    .AsNoTracking()
                    .Where(profile => profile.PublicReference != null &&
                                      profile.PublicReference.StartsWith("RQ-"))
                    .Select(profile => profile.PublicReference),
                "RQ-",
                defaultNumber,
                cancellationToken);
        }

        public static Task<string> NextPropertyReferenceAsync(
            MedriDbContext dbContext,
            CancellationToken cancellationToken)
        {
            return NextAsync(
                dbContext.PropertyListings
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .Where(listing => listing.InternalReference != null &&
                                      listing.InternalReference.StartsWith("IM-"))
                    .Select(listing => listing.InternalReference),
                "IM-",
                defaultNumber: 0,
                cancellationToken);
        }

        private static async Task<string> NextAsync(
            IQueryable<string> references,
            string prefix,
            int defaultNumber,
            CancellationToken cancellationToken)
        {
            var existingReferences = await references.ToArrayAsync(cancellationToken);
            var nextNumber = existingReferences
                .Select(reference => ParseReferenceNumber(reference, prefix))
                .DefaultIfEmpty(defaultNumber)
                .Max() + 1;

            return $"{prefix}{nextNumber.ToString("0000", CultureInfo.InvariantCulture)}";
        }

        private static int ParseReferenceNumber(string reference, string prefix)
        {
            if (string.IsNullOrWhiteSpace(reference) ||
                !reference.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            return int.TryParse(
                reference.Substring(prefix.Length),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var value)
                ? value
                : 0;
        }
    }

    internal static class AdminNavigationCounts
    {
        public static Task<int> UnpublishedListingsAsync(
            MedriDbContext dbContext,
            CancellationToken cancellationToken)
        {
            return dbContext.PropertyListings
                .IgnoreQueryFilters()
                .AsNoTracking()
                .CountAsync(
                    listing => listing.InternalReference != null &&
                               listing.PublicationStatus != PropertyPublicationStatuses.Published &&
                               listing.PublicationStatus != PropertyPublicationStatuses.Archived,
                    cancellationToken);
        }
    }

    internal static class LeadQualificationCalculator
    {
        public static int Calculate(
            Lead lead,
            LeadPreference preference)
        {
            var score = 0;

            if (!string.IsNullOrWhiteSpace(lead.Phone) ||
                !string.IsNullOrWhiteSpace(lead.Email))
            {
                score += 25;
            }

            if (!string.IsNullOrWhiteSpace(preference?.DesiredLocation) ||
                !string.IsNullOrWhiteSpace(preference?.LinkedPropertyReference))
            {
                score += 25;
            }

            if (!string.IsNullOrWhiteSpace(preference?.ValuationGoal))
            {
                score += 25;
            }

            if (lead.AssignedAgencyUserId.HasValue)
            {
                score += 25;
            }

            return score;
        }
    }

    internal static class FeaturedPropertySlots
    {
        public const int MaxSlots = 3;

        public static Task<PropertyListing[]> ActiveAsync(
            MedriDbContext dbContext,
            CancellationToken cancellationToken)
        {
            return dbContext.PropertyListings
                .IgnoreQueryFilters()
                .Where(listing => listing.FeaturedSortOrder.HasValue &&
                                  listing.PublicationStatus == PropertyPublicationStatuses.Published)
                .OrderBy(listing => listing.FeaturedSortOrder.Value)
                .ThenBy(listing => listing.SortOrder)
                .ToArrayAsync(cancellationToken);
        }

        public static async Task NormalizeAsync(
            MedriDbContext dbContext,
            CancellationToken cancellationToken)
        {
            var featured = await ActiveAsync(dbContext, cancellationToken);

            for (var index = 0; index < featured.Length; index++)
            {
                featured[index].FeaturedSortOrder = index + 1;
            }
        }
    }
}
