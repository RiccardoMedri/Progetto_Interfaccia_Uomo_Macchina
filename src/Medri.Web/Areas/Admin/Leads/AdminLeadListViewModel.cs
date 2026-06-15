using System;
using System.Collections.Generic;
using Medri.Services.Medri.Application;
using Medri.Web.Areas.Admin;
using System.Linq;

namespace Medri.Web.Areas.Admin.Leads
{
    public class AdminLeadListInputModel
    {
        public string SearchTerm { get; set; }

        public string Status { get; set; }

        public string RequestType { get; set; }

        public string[] RequestTypes { get; set; } =
            Array.Empty<string>();

        public string Advisor { get; set; }

        public string[] Advisors { get; set; } =
            Array.Empty<string>();

        public string Priority { get; set; }

        public string[] Priorities { get; set; } =
            Array.Empty<string>();

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 15;

        public AdminLeadListQueryFilter ToQueryFilter()
        {
            var filter = new AdminLeadListQueryFilter
            {
                SearchTerm = SearchTerm,
                Statuses = SelectedStatuses(),
                RequestTypes = SelectedRequestTypes(),
                Priorities = SelectedPriorities(),
                OnlyUnassigned = Advisor == "unassigned",
                Page = Page,
                PageSize = PageSize
            };

            filter.AssignedAgencyUserIds = SelectedAdvisors()
                .Where(value => Guid.TryParse(value, out _))
                .Select(Guid.Parse)
                .ToArray();
            filter.OnlyUnassigned = SelectedAdvisors()
                .Any(value => value == "unassigned");

            return filter;
        }

        public IReadOnlyList<string> SelectedStatuses()
        {
            return SplitStatuses(Status);
        }

        public IReadOnlyList<string> SelectedRequestTypes()
        {
            return SplitValues(RequestType)
                .Concat(RequestTypes ?? Array.Empty<string>())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public IReadOnlyList<string> SelectedPriorities()
        {
            return SplitValues(Priority)
                .Concat(Priorities ?? Array.Empty<string>())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public IReadOnlyList<string> SelectedAdvisors()
        {
            return SplitValues(Advisor)
                .Concat(Advisors ?? Array.Empty<string>())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public static string JoinStatuses(IEnumerable<string> statuses)
        {
            var values = (statuses ?? Array.Empty<string>())
                .Where(status => !string.IsNullOrWhiteSpace(status))
                .Select(status => status.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return values.Length == 0 ? null : string.Join(",", values);
        }

        private static IReadOnlyList<string> SplitStatuses(string status)
        {
            return SplitValues(status);
        }

        private static IReadOnlyList<string> SplitValues(string status)
        {
            return (status ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
    }

    public class AdminLeadListReturnInputModel : AdminLeadListInputModel
    {
        public string ReturnTo { get; set; }

        public bool ShouldReturnToIndex => ReturnTo == "Index";

        public object ToRouteValues()
        {
            return new
            {
                SearchTerm,
                Status,
                RequestType,
                Advisor,
                Priority,
                Page,
                PageSize
            };
        }
    }

    public sealed class AdminLeadBulkActionInputModel : AdminLeadListReturnInputModel
    {
        public string[] SelectedLeadReferences { get; set; } =
            Array.Empty<string>();

        public Guid? BulkAssignedAgencyUserId { get; set; }
    }

    public sealed class AdminLeadListViewModel : IAdminPageViewModel
    {
        public AdminNavigationViewModel Navigation { get; set; }

        public string SearchTerm { get; set; }

        public string Status { get; set; }

        public string RequestType { get; set; }

        public string Advisor { get; set; }

        public string Priority { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }

        public IReadOnlyList<int> PageSizeOptions { get; set; } =
            Array.Empty<int>();

        public IReadOnlyList<AdminLeadStatusCardViewModel> StatusCards { get; set; } =
            Array.Empty<AdminLeadStatusCardViewModel>();

        public IReadOnlyList<AdminLeadOptionViewModel> RequestTypeOptions { get; set; } =
            Array.Empty<AdminLeadOptionViewModel>();

        public IReadOnlyList<AdminLeadOptionViewModel> PriorityOptions { get; set; } =
            Array.Empty<AdminLeadOptionViewModel>();

        public IReadOnlyList<AdminLeadOptionViewModel> AdvisorOptions { get; set; } =
            Array.Empty<AdminLeadOptionViewModel>();

        public IReadOnlyList<AdminLeadRowViewModel> Leads { get; set; } =
            Array.Empty<AdminLeadRowViewModel>();

        public bool HasLeads => Leads.Count > 0;

        public bool HasPreviousPage => Page > 1;

        public bool HasNextPage => Page < TotalPages;
    }

    public sealed class AdminLeadStatusCardViewModel
    {
        public string Status { get; set; }

        public string Label { get; set; }

        public int Count { get; set; }

        public string Description { get; set; }

        public bool IsSelected { get; set; }

        public string ToggledStatusFilter { get; set; }
    }

    public sealed class AdminLeadOptionViewModel
    {
        public string Value { get; set; }

        public string Label { get; set; }

        public bool IsSelected { get; set; }
    }

    public sealed class AdminLeadRowViewModel
    {
        public string Reference { get; set; }

        public string FullName { get; set; }

        public string ContactSummary { get; set; }

        public string SourceLabel { get; set; }

        public string NeedLabel { get; set; }

        public string StatusLabel { get; set; }

        public string StatusClass { get; set; }

        public int QualificationPercent { get; set; }

        public string PriorityLabel { get; set; }

        public string PriorityClass { get; set; }

        public string AdvisorLabel { get; set; }

        public bool CanConvert { get; set; }

        public bool CanArchive { get; set; }
    }
}
