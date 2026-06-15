using System.Security.Claims;
using System.Linq;

namespace Medri.Web.Areas.Admin
{
    public interface IAdminPageViewModel
    {
        AdminNavigationViewModel Navigation { get; }
    }

    public sealed class AdminNavigationViewModel
    {
        public string ActiveSection { get; set; }

        public string DisplayName { get; set; }

        public string RoleLabel { get; set; }

        public string Initials { get; set; }

        public int LeadCount { get; set; }

        public int RequestCount { get; set; }

        public int ListingCount { get; set; }

        public bool IsActive(string section)
        {
            return ActiveSection == section;
        }
    }

    public static class AdminSections
    {
        public const string Dashboard = "Dashboard";
        public const string Leads = "Leads";
        public const string Requests = "Requests";
        public const string Properties = "Properties";
    }

    internal static class AdminNavigationViewModelMapper
    {
        public static AdminNavigationViewModel Create(
            ClaimsPrincipal user,
            int leadCount,
            int requestCount,
            int listingCount,
            string activeSection)
        {
            var displayName = user?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(displayName))
            {
                displayName = user?.FindFirstValue(ClaimTypes.Email) ?? "Operatore";
            }

            return new AdminNavigationViewModel
            {
                ActiveSection = activeSection,
                DisplayName = displayName,
                RoleLabel = RoleLabel(user?.FindFirstValue(ClaimTypes.Role)),
                Initials = Initials(displayName),
                LeadCount = leadCount,
                RequestCount = requestCount,
                ListingCount = listingCount
            };
        }

        private static string RoleLabel(string role)
        {
            return role == "Admin" ? "Responsabile agenzia" : "Operatore";
        }

        private static string Initials(string displayName)
        {
            var words = displayName
                .Split(' ', System.StringSplitOptions.RemoveEmptyEntries)
                .Take(2)
                .ToArray();

            return words.Length == 0
                ? "OP"
                : string.Concat(words.Select(word => char.ToUpperInvariant(word[0])));
        }
    }
}
