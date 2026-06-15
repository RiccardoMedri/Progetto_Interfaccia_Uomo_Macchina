using System.Linq;
using System.Security.Claims;
using Medri.Services.Medri.Application;
using Medri.Web.Areas.Admin;

namespace Medri.Web.Areas.Admin.Dashboard
{
    internal static class AdminDashboardViewModelMapper
    {
        public static AdminDashboardViewModel Create(
            AdminDashboardResultDto result,
            ClaimsPrincipal user)
        {
            return new AdminDashboardViewModel
            {
                Navigation = AdminNavigationViewModelMapper.Create(
                    user,
                    result.LeadWorkCount,
                    result.ActiveRequestCount,
                    result.ListingCount,
                    AdminSections.Dashboard),
                LeadWorkCount = result.LeadWorkCount,
                ActiveRequestCount = result.ActiveRequestCount,
                IncompleteListingCount = result.IncompleteListingCount,
                RecentLeads = result.RecentLeads
                    .Select(lead => new AdminDashboardLeadViewModel
                    {
                        Reference = lead.Reference,
                        ContactName = lead.ContactName,
                        IntentLabel = RequestTypeLabel(lead.RequestType),
                        AdvisorLabel = AdvisorLabel(lead.AdvisorDisplayName)
                    })
                    .ToArray(),
                ActiveRequests = result.ActiveRequests
                    .Select(request => new AdminDashboardRequestViewModel
                    {
                        Reference = request.Reference,
                        CustomerName = request.CustomerName,
                        StatusLabel = RequestStatusLabel(request.Status),
                        StatusClass = RequestStatusClass(request.Status),
                        AdvisorLabel = AdvisorLabel(request.AdvisorDisplayName)
                    })
                    .ToArray(),
                Listings = result.Listings
                    .Select(listing => new AdminDashboardListingViewModel
                    {
                        Reference = listing.Reference,
                        Title = listing.Title,
                        StatusLabel = ListingStatusLabel(listing.Status),
                        StatusClass = ListingStatusClass(listing.Status),
                        AdvisorLabel = AdvisorLabel(listing.AdvisorDisplayName)
                    })
                    .ToArray()
            };
        }

        private static string RequestTypeLabel(string requestType)
        {
            return requestType switch
            {
                "Valuation" => "Valutazione",
                "Rent" => "Affitto",
                "Sell" => "Vendita",
                "RentOut" => "Locazione",
                _ => "Acquisto"
            };
        }

        private static string RequestStatusLabel(string status)
        {
            return status switch
            {
                "InMatching" => "In matching",
                "New" => "Nuova",
                "Updating" => "In aggiornamento",
                _ => "Attiva"
            };
        }

        private static string RequestStatusClass(string status)
        {
            return status switch
            {
                "InMatching" => "is-matching",
                "New" => "is-new",
                "Updating" => "is-warning",
                _ => "is-active"
            };
        }

        private static string ListingStatusLabel(string status)
        {
            return status switch
            {
                "Incomplete" => "Incompleto",
                "NeedsUpdate" => "Da aggiornare",
                "Ready" => "Pronto",
                _ => "Pubblicato"
            };
        }

        private static string ListingStatusClass(string status)
        {
            return status switch
            {
                "Ready" => "is-success",
                "NeedsUpdate" => "is-update-needed",
                _ => "is-warning"
            };
        }

        private static string AdvisorLabel(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return "Non assegnato";
            }

            return displayName
                .Split(' ', System.StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault() ?? displayName;
        }

    }
}
