using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Medri.Services.Medri.Application;
using Medri.Web.Areas.Admin;

namespace Medri.Web.Areas.Admin.Leads
{
    internal static class AdminLeadListViewModelMapper
    {
        public static AdminLeadListViewModel Create(
            AdminLeadListResultDto result,
            AdminLeadListInputModel input,
            ClaimsPrincipal user)
        {
            input ??= new AdminLeadListInputModel();

            return new AdminLeadListViewModel
            {
                Navigation = AdminNavigationViewModelMapper.Create(
                    user,
                    result.NewCount,
                    result.ActiveRequestCount,
                    result.ListingCount,
                    AdminSections.Leads),
                SearchTerm = input.SearchTerm,
                Status = input.Status,
                RequestType = AdminLeadListInputModel.JoinStatuses(input.SelectedRequestTypes()),
                Advisor = AdminLeadListInputModel.JoinStatuses(input.SelectedAdvisors()),
                Priority = AdminLeadListInputModel.JoinStatuses(input.SelectedPriorities()),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems,
                TotalPages = TotalPages(result.TotalItems, result.PageSize),
                PageSizeOptions = new[] { 15, 25, 50 },
                StatusCards = CreateStatusCards(result, input.SelectedStatuses()),
                RequestTypeOptions = CreateRequestTypeOptions(input.SelectedRequestTypes()),
                PriorityOptions = CreatePriorityOptions(input.SelectedPriorities()),
                AdvisorOptions = CreateAdvisorOptions(result.Advisors, input.SelectedAdvisors()),
                Leads = result.Leads.Select(CreateRow).ToArray()
            };
        }

        private static IReadOnlyList<AdminLeadStatusCardViewModel> CreateStatusCards(
            AdminLeadListResultDto result,
            IReadOnlyList<string> selectedStatuses)
        {
            return new[]
            {
                new AdminLeadStatusCardViewModel
                {
                    Status = "New",
                    Label = "Nuovi",
                    Count = result.NewCount,
                    Description = "Contatti appena registrati, ancora da capire e assegnare correttamente.",
                    IsSelected = selectedStatuses.Contains("New"),
                    ToggledStatusFilter = ToggleStatus(selectedStatuses, "New")
                },
                new AdminLeadStatusCardViewModel
                {
                    Status = "InContact",
                    Label = "In contatto",
                    Count = result.InContactCount,
                    Description = "Persone gia sentite almeno una volta, con prossima azione da seguire.",
                    IsSelected = selectedStatuses.Contains("InContact"),
                    ToggledStatusFilter = ToggleStatus(selectedStatuses, "InContact")
                },
                new AdminLeadStatusCardViewModel
                {
                    Status = "Archived",
                    Label = "Archiviati",
                    Count = result.ArchivedCount,
                    Description = "Contatti non coltivabili o non piu interessati, mantenuti come storico.",
                    IsSelected = selectedStatuses.Contains("Archived"),
                    ToggledStatusFilter = ToggleStatus(selectedStatuses, "Archived")
                }
            };
        }

        private static IReadOnlyList<AdminLeadOptionViewModel> CreateRequestTypeOptions(
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

        private static IReadOnlyList<AdminLeadOptionViewModel> CreatePriorityOptions(
            IReadOnlyList<string> selected)
        {
            return new[]
            {
                CreateOption(null, "Tutte", selected),
                CreateOption("Alta", "Alta", selected),
                CreateOption("Media", "Media", selected),
                CreateOption("Bassa", "Bassa", selected),
                CreateOption("Da capire", "Da capire", selected)
            };
        }

        private static IReadOnlyList<AdminLeadOptionViewModel> CreateAdvisorOptions(
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

        private static AdminLeadOptionViewModel CreateOption(
            string value,
            string label,
            IReadOnlyList<string> selected)
        {
            return new AdminLeadOptionViewModel
            {
                Value = value,
                Label = label,
                IsSelected = string.IsNullOrWhiteSpace(value)
                    ? selected.Count == 0
                    : selected.Contains(value)
            };
        }

        private static AdminLeadRowViewModel CreateRow(AdminLeadRowDto lead)
        {
            return new AdminLeadRowViewModel
            {
                Reference = lead.Reference,
                FullName = lead.FullName,
                ContactSummary = ContactSummary(lead),
                SourceLabel = lead.SourceChannel,
                NeedLabel = RequestTypeLabel(lead.RequestType),
                StatusLabel = StatusLabel(lead.WorkflowStatus),
                StatusClass = StatusClass(lead.WorkflowStatus),
                PriorityLabel = PriorityLabel(lead.Priority),
                PriorityClass = PriorityClass(lead.Priority),
                QualificationPercent = lead.QualificationPercent,
                AdvisorLabel = string.IsNullOrWhiteSpace(lead.AdvisorDisplayName)
                    ? "Non assegnato"
                    : FirstName(lead.AdvisorDisplayName),
                CanConvert = lead.WorkflowStatus != "Archived" && lead.WorkflowStatus != "Qualified",
                CanArchive = lead.WorkflowStatus != "Archived"
            };
        }

        private static string ContactSummary(AdminLeadRowDto lead)
        {
            if (!string.IsNullOrWhiteSpace(lead.Phone) && !string.IsNullOrWhiteSpace(lead.Email))
            {
                return $"{lead.Phone} - {lead.Email}";
            }

            if (!string.IsNullOrWhiteSpace(lead.Phone))
            {
                return lead.SourceChannel == "WhatsApp"
                    ? $"{lead.Phone} - WhatsApp"
                    : lead.Phone;
            }

            if (!string.IsNullOrWhiteSpace(lead.Email))
            {
                return lead.Email;
            }

            return lead.SourceChannel == "Ufficio"
                ? "In ufficio - recapito da completare"
                : "Recapito da completare";
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

        private static string StatusLabel(string status)
        {
            return status switch
            {
                "InContact" => "In contatto",
                "Archived" => "Archiviato",
                "Qualified" => "Qualificato",
                _ => "Nuovo"
            };
        }

        private static string StatusClass(string status)
        {
            return status switch
            {
                "InContact" => "is-contact",
                "Archived" => "is-low",
                "Qualified" => "is-success",
                _ => "is-new"
            };
        }

        private static string PriorityLabel(string priority)
        {
            return string.IsNullOrWhiteSpace(priority) ? "Da capire" : priority;
        }

        private static string PriorityClass(string priority)
        {
            return PriorityLabel(priority) switch
            {
                "Alta" => "is-priority",
                "Bassa" => "is-low",
                _ => string.Empty
            };
        }

        private static string FirstName(string displayName)
        {
            return displayName
                .Split(' ', System.StringSplitOptions.RemoveEmptyEntries)
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

            return AdminLeadListInputModel.JoinStatuses(values);
        }

        private static int TotalPages(int totalItems, int pageSize)
        {
            return System.Math.Max(1, (int)System.Math.Ceiling(totalItems / (double)pageSize));
        }
    }
}
