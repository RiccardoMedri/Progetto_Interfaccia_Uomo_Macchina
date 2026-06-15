using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Medri.Services.Medri.Application;
using Medri.Web.Areas.Admin;

namespace Medri.Web.Areas.Admin.Requests
{
    internal static class AdminRequestListViewModelMapper
    {
        public static AdminRequestListViewModel Create(
            AdminRequestListResultDto result,
            AdminRequestListInputModel input,
            ClaimsPrincipal user)
        {
            input ??= new AdminRequestListInputModel();

            return new AdminRequestListViewModel
            {
                Navigation = AdminNavigationViewModelMapper.Create(
                    user,
                    result.LeadCount,
                    result.ActiveRequestCount,
                    result.ListingCount,
                    AdminSections.Requests),
                SearchTerm = input.SearchTerm,
                RequestType = AdminRequestListInputModel.JoinValues(input.SelectedRequestTypes()),
                Status = AdminRequestListInputModel.JoinValues(input.SelectedStatuses()),
                Advisor = AdminRequestListInputModel.JoinValues(input.SelectedAdvisors()),
                Priority = AdminRequestListInputModel.JoinValues(input.SelectedPriorities()),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems,
                TotalPages = TotalPages(result.TotalItems, result.PageSize),
                PageSizeOptions = new[] { 15, 25, 50 },
                StatusCards = CreateStatusCards(result, input.SelectedStatuses()),
                RequestTypeOptions = CreateRequestTypeOptions(input.SelectedRequestTypes()),
                AdvisorOptions = CreateAdvisorOptions(result.Advisors, input.SelectedAdvisors()),
                PriorityOptions = CreatePriorityOptions(input.SelectedPriorities()),
                Requests = result.Requests
                    .Select(CreateRow)
                    .ToArray()
            };
        }

        private static IReadOnlyList<AdminRequestStatusCardViewModel> CreateStatusCards(
            AdminRequestListResultDto result,
            IReadOnlyList<string> selectedStatuses)
        {
            return new[]
            {
                new AdminRequestStatusCardViewModel
                {
                    Status = "New",
                    Label = "Nuove",
                    Count = result.NewCount,
                    Description = "Richieste appena ricevute, ancora senza referente o primo controllo interno.",
                    IsSelected = selectedStatuses.Contains("New"),
                    ToggledStatusFilter = ToggleStatus(selectedStatuses, "New")
                },
                new AdminRequestStatusCardViewModel
                {
                    Status = "Assigned",
                    Label = "Assegnate",
                    Count = result.AssignedCount,
                    Description = "Richieste con referente definito e prossima azione gia pianificata.",
                    IsSelected = selectedStatuses.Contains("Assigned"),
                    ToggledStatusFilter = ToggleStatus(selectedStatuses, "Assigned")
                },
                new AdminRequestStatusCardViewModel
                {
                    Status = "Updating",
                    Label = "In aggiornamento",
                    Count = result.UpdatingCount,
                    Description = "Preferenze o priorita cambiate dopo telefonate, appuntamenti o visite.",
                    IsSelected = selectedStatuses.Contains("Updating"),
                    ToggledStatusFilter = ToggleStatus(selectedStatuses, "Updating")
                },
                new AdminRequestStatusCardViewModel
                {
                    Status = "InMatching",
                    Label = "In matching",
                    Count = result.MatchingCount,
                    Description = "Profili abbastanza chiari per proporre immobili pieni, parziali o potenziali.",
                    IsSelected = selectedStatuses.Contains("InMatching"),
                    ToggledStatusFilter = ToggleStatus(selectedStatuses, "InMatching")
                }
            };
        }

        private static IReadOnlyList<AdminRequestOptionViewModel> CreateRequestTypeOptions(
            IReadOnlyList<string> selected)
        {
            return new[]
            {
                CreateOption(null, "Tutte", selected),
                CreateOption("Buy", "Compra", selected),
                CreateOption("Rent", "Affitta", selected),
                CreateOption("Sell", "Vende", selected),
                CreateOption("Valuation", "Valuta", selected)
            };
        }

        private static IReadOnlyList<AdminRequestOptionViewModel> CreateAdvisorOptions(
            IReadOnlyList<AdminLeadAdvisorDto> advisors,
            IReadOnlyList<string> selected)
        {
            var orderedAdvisors = advisors
                .OrderBy(advisor => advisor.DisplayName);

            return new[] { CreateOption(null, "Tutti", selected) }
                .Concat(orderedAdvisors.Select(advisor =>
                    CreateOption(advisor.Id.ToString(), FirstName(advisor.DisplayName), selected)))
                .Concat(new[] { CreateOption("unassigned", "Non assegnato", selected) })
                .ToArray();
        }

        private static IReadOnlyList<AdminRequestOptionViewModel> CreatePriorityOptions(
            IReadOnlyList<string> selected)
        {
            return new[]
            {
                CreateOption(null, "Tutte", selected),
                CreateOption("Alta", "Alta", selected),
                CreateOption("Media", "Media", selected),
                CreateOption("Bassa", "Bassa", selected)
            };
        }

        private static AdminRequestOptionViewModel CreateOption(
            string value,
            string label,
            IReadOnlyList<string> selected)
        {
            return new AdminRequestOptionViewModel
            {
                Value = value,
                Label = label,
                IsSelected = string.IsNullOrWhiteSpace(value)
                    ? selected.Count == 0
                    : selected.Contains(value)
            };
        }

        private static AdminRequestRowViewModel CreateRow(AdminRequestRowDto request)
        {
            return new AdminRequestRowViewModel
            {
                Reference = request.Reference,
                CustomerName = request.CustomerName,
                ContactSummary = request.ContactSummary,
                RequestTypeLabel = RequestTypeLabel(request.RequestType),
                StatusLabel = StatusLabel(request.Status, request.IsAssigned),
                StatusClass = StatusClass(request.Status, request.IsAssigned),
                CompletionPercent = request.CompletionPercent,
                PriorityLabel = PriorityLabel(request.Priority),
                PriorityClass = PriorityClass(request.Priority),
                AdvisorLabel = string.IsNullOrWhiteSpace(request.AdvisorDisplayName)
                    ? "Non assegnato"
                    : FirstName(request.AdvisorDisplayName)
            };
        }

        private static string RequestTypeLabel(string requestType)
        {
            return requestType switch
            {
                "Valuation" => "Valuta",
                "Rent" => "Affitta",
                "Sell" => "Vende",
                "RentOut" => "Affida",
                _ => "Compra"
            };
        }

        private static string StatusLabel(string status, bool isAssigned)
        {
            if (status == "New" && isAssigned)
            {
                return "Assegnata";
            }

            return status switch
            {
                "InMatching" => "In matching",
                "Updating" => "In aggiornamento",
                "Archived" => "Archiviata",
                _ => "Nuova"
            };
        }

        private static string StatusClass(string status, bool isAssigned)
        {
            if (status == "New" && isAssigned)
            {
                return "is-assigned";
            }

            return status switch
            {
                "InMatching" => "is-matching",
                "Updating" => "is-warning",
                "Archived" => "is-low",
                _ => "is-new"
            };
        }

        private static string PriorityLabel(string priority)
        {
            return string.IsNullOrWhiteSpace(priority) ? "Da capire" : priority;
        }

        private static string PriorityClass(string priority)
        {
            return priority switch
            {
                "Alta" => "is-priority",
                "Bassa" => "is-low",
                _ => string.Empty
            };
        }

        private static string FirstName(string displayName)
        {
            return displayName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault() ?? displayName;
        }

        private static string ToggleStatus(
            IReadOnlyList<string> selectedStatuses,
            string status)
        {
            var values = selectedStatuses.ToList();
            var existing = values.FirstOrDefault(
                item => item == status);

            if (existing == null)
            {
                values.Add(status);
            }
            else
            {
                values.Remove(existing);
            }

            return AdminRequestListInputModel.JoinValues(values);
        }

        private static int TotalPages(int totalItems, int pageSize)
        {
            return Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        }
    }
}
