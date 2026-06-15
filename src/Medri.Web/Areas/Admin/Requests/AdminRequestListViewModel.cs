using System;
using System.Collections.Generic;
using System.Linq;
using Medri.Services.Medri.Application;
using Medri.Web.Areas.Admin;

namespace Medri.Web.Areas.Admin.Requests
{
    public class AdminRequestListInputModel
    {
        public string SearchTerm { get; set; }

        public string RequestType { get; set; }

        public string[] RequestTypes { get; set; } =
            Array.Empty<string>();

        public string Status { get; set; }

        public string[] Statuses { get; set; } =
            Array.Empty<string>();

        public string Advisor { get; set; }

        public string[] Advisors { get; set; } =
            Array.Empty<string>();

        public string Priority { get; set; }

        public string[] Priorities { get; set; } =
            Array.Empty<string>();

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 15;

        public AdminRequestListQueryFilter ToQueryFilter()
        {
            var filter = new AdminRequestListQueryFilter
            {
                SearchTerm = SearchTerm,
                RequestTypes = SelectedRequestTypes(),
                Statuses = SelectedStatuses(),
                Priorities = SelectedPriorities(),
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

        public IReadOnlyList<string> SelectedRequestTypes()
        {
            return SplitValues(RequestType, RequestTypes);
        }

        public IReadOnlyList<string> SelectedStatuses()
        {
            return SplitValues(Status, Statuses);
        }

        public IReadOnlyList<string> SelectedAdvisors()
        {
            return SplitValues(Advisor, Advisors);
        }

        public IReadOnlyList<string> SelectedPriorities()
        {
            return SplitValues(Priority, Priorities);
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
                RequestType = JoinValues(SelectedRequestTypes()),
                Status = JoinValues(SelectedStatuses()),
                Advisor = JoinValues(SelectedAdvisors()),
                Priority = JoinValues(SelectedPriorities()),
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

    public sealed class AdminRequestListViewModel : IAdminPageViewModel
    {
        public AdminNavigationViewModel Navigation { get; set; }

        public string SearchTerm { get; set; }

        public string RequestType { get; set; }

        public string Status { get; set; }

        public string Advisor { get; set; }

        public string Priority { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }

        public IReadOnlyList<int> PageSizeOptions { get; set; } =
            Array.Empty<int>();

        public IReadOnlyList<AdminRequestStatusCardViewModel> StatusCards { get; set; } =
            Array.Empty<AdminRequestStatusCardViewModel>();

        public IReadOnlyList<AdminRequestOptionViewModel> RequestTypeOptions { get; set; } =
            Array.Empty<AdminRequestOptionViewModel>();

        public IReadOnlyList<AdminRequestOptionViewModel> AdvisorOptions { get; set; } =
            Array.Empty<AdminRequestOptionViewModel>();

        public IReadOnlyList<AdminRequestOptionViewModel> PriorityOptions { get; set; } =
            Array.Empty<AdminRequestOptionViewModel>();

        public IReadOnlyList<AdminRequestRowViewModel> Requests { get; set; } =
            Array.Empty<AdminRequestRowViewModel>();

        public bool HasRequests => Requests.Count > 0;

        public bool HasPreviousPage => Page > 1;

        public bool HasNextPage => Page < TotalPages;
    }

    public sealed class AdminRequestStatusCardViewModel
    {
        public string Status { get; set; }

        public string Label { get; set; }

        public int Count { get; set; }

        public string Description { get; set; }

        public bool IsSelected { get; set; }

        public string ToggledStatusFilter { get; set; }
    }

    public sealed class AdminRequestOptionViewModel
    {
        public string Value { get; set; }

        public string Label { get; set; }

        public bool IsSelected { get; set; }
    }

    public sealed class AdminRequestRowViewModel
    {
        public string Reference { get; set; }

        public string CustomerName { get; set; }

        public string ContactSummary { get; set; }

        public string RequestTypeLabel { get; set; }

        public string StatusLabel { get; set; }

        public string StatusClass { get; set; }

        public int CompletionPercent { get; set; }

        public string PriorityLabel { get; set; }

        public string PriorityClass { get; set; }

        public string AdvisorLabel { get; set; }
    }

    public sealed class AdminRequestBulkActionInputModel : AdminRequestListInputModel
    {
        public string[] SelectedRequestReferences { get; set; } =
            Array.Empty<string>();

        public Guid? BulkAssignedAgencyUserId { get; set; }
    }
}
