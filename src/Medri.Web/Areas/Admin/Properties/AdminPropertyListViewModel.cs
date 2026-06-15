using System;
using System.Collections.Generic;
using System.Linq;
using Medri.Services.Medri.Application;
using Medri.Web.Areas.Admin;

namespace Medri.Web.Areas.Admin.Properties
{
    public class AdminPropertyListInputModel
    {
        public string SearchTerm { get; set; }

        public string Status { get; set; }

        public string[] Statuses { get; set; } =
            Array.Empty<string>();

        public string Contract { get; set; }

        public string[] Contracts { get; set; } =
            Array.Empty<string>();

        public string Advisor { get; set; }

        public string[] Advisors { get; set; } =
            Array.Empty<string>();

        public string ReturnTo { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 15;

        public AdminPropertyListQueryFilter ToQueryFilter()
        {
            var filter = new AdminPropertyListQueryFilter
            {
                SearchTerm = SearchTerm,
                Statuses = SelectedStatuses(),
                Contracts = SelectedContracts(),
                OnlyUnassigned = SelectedAdvisors().Any(value => value == "unassigned"),
                Page = Page,
                PageSize = PageSize
            };

            filter.AssignedAgencyUserIds = SelectedAdvisors()
                .Where(value => Guid.TryParse(value, out _))
                .Select(Guid.Parse)
                .ToArray();

            return filter;
        }

        public IReadOnlyList<string> SelectedStatuses()
        {
            return SplitValues(Status, Statuses);
        }

        public IReadOnlyList<string> SelectedContracts()
        {
            return SplitValues(Contract, Contracts);
        }

        public IReadOnlyList<string> SelectedAdvisors()
        {
            return SplitValues(Advisor, Advisors);
        }

        public static string JoinValues(IEnumerable<string> values)
        {
            var selected = (values ?? Array.Empty<string>())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return selected.Length == 0 ? null : string.Join(",", selected);
        }

        public object ToRouteValues()
        {
            return new
            {
                SearchTerm,
                Status = JoinValues(SelectedStatuses()),
                Contract = JoinValues(SelectedContracts()),
                Advisor = JoinValues(SelectedAdvisors()),
                Page,
                PageSize
            };
        }

        private static IReadOnlyList<string> SplitValues(
            string scalar,
            IEnumerable<string> repeated)
        {
            return (scalar ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Concat(repeated ?? Array.Empty<string>())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
    }

    public sealed class AdminPropertyListViewModel : IAdminPageViewModel
    {
        public AdminNavigationViewModel Navigation { get; set; }

        public string SearchTerm { get; set; }

        public string Status { get; set; }

        public string Contract { get; set; }

        public string Advisor { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }

        public IReadOnlyList<int> PageSizeOptions { get; set; } =
            Array.Empty<int>();

        public IReadOnlyList<AdminPropertyStatusCardViewModel> StatusCards { get; set; } =
            Array.Empty<AdminPropertyStatusCardViewModel>();

        public IReadOnlyList<AdminPropertyOptionViewModel> StatusOptions { get; set; } =
            Array.Empty<AdminPropertyOptionViewModel>();

        public IReadOnlyList<AdminPropertyOptionViewModel> ContractOptions { get; set; } =
            Array.Empty<AdminPropertyOptionViewModel>();

        public IReadOnlyList<AdminPropertyOptionViewModel> AdvisorOptions { get; set; } =
            Array.Empty<AdminPropertyOptionViewModel>();

        public IReadOnlyList<AdminPropertyRowViewModel> Properties { get; set; } =
            Array.Empty<AdminPropertyRowViewModel>();

        public IReadOnlyList<AdminPropertyFeaturedSlotViewModel> FeaturedSlots { get; set; } =
            Array.Empty<AdminPropertyFeaturedSlotViewModel>();

        public bool HasProperties => Properties.Count > 0;

        public bool HasFeaturedSlots => FeaturedSlots.Count > 0;

        public bool HasPreviousPage => Page > 1;

        public bool HasNextPage => Page < TotalPages;
    }

    public sealed class AdminPropertyStatusCardViewModel
    {
        public string Status { get; set; }

        public string Label { get; set; }

        public int Count { get; set; }

        public string Description { get; set; }

        public bool IsSelected { get; set; }

        public string ToggledStatusFilter { get; set; }
    }

    public sealed class AdminPropertyOptionViewModel
    {
        public string Value { get; set; }

        public string Label { get; set; }

        public bool IsSelected { get; set; }
    }

    public sealed class AdminPropertyRowViewModel
    {
        public string Reference { get; set; }

        public string Title { get; set; }

        public string DisplayLocation { get; set; }

        public string ThumbnailUrl { get; set; }

        public string ContractLabel { get; set; }

        public string StatusLabel { get; set; }

        public string StatusClass { get; set; }

        public int CompletionPercent { get; set; }

        public string MissingItems { get; set; }

        public string AdvisorLabel { get; set; }

        public bool IsFeatured { get; set; }

        public bool CanFeature { get; set; }

        public bool CanArchive { get; set; }
    }

    public sealed class AdminPropertyFeaturedSlotViewModel
    {
        public int SlotNumber { get; set; }

        public string Reference { get; set; }

        public string Title { get; set; }

        public string DisplayLocation { get; set; }
    }

    public sealed class AdminPropertyBulkActionInputModel : AdminPropertyListInputModel
    {
        public string[] SelectedPropertyReferences { get; set; } =
            Array.Empty<string>();

        public Guid? BulkAssignedAgencyUserId { get; set; }
    }
}
