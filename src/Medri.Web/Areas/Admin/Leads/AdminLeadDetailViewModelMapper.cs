using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Medri.Services.Medri.Application;
using Medri.Web.Areas.Admin;

namespace Medri.Web.Areas.Admin.Leads
{
    internal static class AdminLeadDetailViewModelMapper
    {
        public static AdminLeadDetailViewModel Create(
            AdminLeadDetailResultDto result,
            AdminLeadDetailInputModel input,
            ClaimsPrincipal user)
        {
            var values = input ?? CreateInput(result);

            var model = new AdminLeadDetailViewModel
            {
                Navigation = AdminNavigationViewModelMapper.Create(
                    user,
                    result.NewCount,
                    result.ActiveRequestCount,
                    result.ListingCount,
                    AdminSections.Leads),
                Reference = result.Reference,
                IsCreateMode = result.IsCreateMode,
                DisplayName = FullName(values.FirstName, values.LastName, result.FullName),
                WorkflowStatus = result.WorkflowStatus,
                StatusLabel = StatusLabel(result.WorkflowStatus),
                StatusClass = StatusClass(result.WorkflowStatus),
                SearchProfileReference = result.SearchProfileReference,
                FirstName = values.FirstName,
                LastName = values.LastName,
                Phone = values.Phone,
                Email = values.Email,
                SourceChannel = values.SourceChannel,
                AssignedAgencyUserId = values.AssignedAgencyUserId,
                RequestType = values.RequestType,
                Urgency = values.Urgency,
                DesiredLocation = values.DesiredLocation,
                ExpectedPriceOrMainQuestion = values.ExpectedPriceOrMainQuestion,
                LinkedPropertyReference = values.LinkedPropertyReference,
                ContactReason = values.ContactReason,
                Notes = values.Notes,
                NextContactQuestions = values.NextContactQuestions,
                PhoneHref = PhoneHref(values.Phone),
                WhatsAppHref = WhatsAppHref(values.Phone),
                MailtoHref = string.IsNullOrWhiteSpace(values.Email) ? null : $"mailto:{values.Email}",
                SourceOptions = CreateSourceOptions(values.SourceChannel),
                AdvisorOptions = CreateAdvisorOptions(result.Advisors, values.AssignedAgencyUserId),
                RequestTypeOptions = CreateRequestTypeOptions(values.RequestType),
                UrgencyOptions = CreateUrgencyOptions(values.Urgency),
                Interactions = CreateInteractions(result.Interactions),
                ConversionChecklist = CreateChecklist(values)
            };

            model.SummaryCards = CreateSummaryCards(result, model);
            return model;
        }

        private static AdminLeadDetailInputModel CreateInput(AdminLeadDetailResultDto result)
        {
            if (result.IsCreateMode)
            {
                return new AdminLeadDetailInputModel
                {
                    SourceChannel = result.SourceChannel,
                    RequestType = result.RequestType
                };
            }

            var name = SplitName(result.FullName);

            return new AdminLeadDetailInputModel
            {
                FirstName = name.FirstName,
                LastName = name.LastName,
                Phone = result.Phone,
                Email = result.Email,
                SourceChannel = result.SourceChannel,
                AssignedAgencyUserId = result.AssignedAgencyUserId,
                RequestType = result.RequestType,
                Urgency = result.Urgency,
                DesiredLocation = result.DesiredLocation,
                ExpectedPriceOrMainQuestion = result.ExpectedPriceOrMainQuestion,
                LinkedPropertyReference = result.LinkedPropertyReference,
                ContactReason = result.ContactReason,
                Notes = result.Notes,
                NextContactQuestions = result.NextContactQuestions
            };
        }

        private static IReadOnlyList<AdminLeadSummaryCardViewModel> CreateSummaryCards(
            AdminLeadDetailResultDto result,
            AdminLeadDetailViewModel model)
        {
            return new[]
            {
                new AdminLeadSummaryCardViewModel
                {
                    Label = "Stato",
                    Value = model.StatusLabel,
                    Description = StatusDescription(result.WorkflowStatus)
                },
                new AdminLeadSummaryCardViewModel
                {
                    Label = "Esigenza",
                    Value = RequestTypeShortLabel(model.RequestType),
                    Description = RequestTypeDescription(model.RequestType)
                },
                new AdminLeadSummaryCardViewModel
                {
                    Label = "Fonte",
                    Value = string.IsNullOrWhiteSpace(model.SourceChannel) ? "Da completare" : model.SourceChannel,
                    Description = SourceDescription(model.SourceChannel)
                },
                new AdminLeadSummaryCardViewModel
                {
                    Label = "Referente",
                    Value = AdvisorSummary(result.Advisors, model.AssignedAgencyUserId),
                    Description = model.AssignedAgencyUserId.HasValue
                        ? "In carico al referente selezionato"
                        : "Da smistare dopo primo controllo"
                }
            };
        }

        private static IReadOnlyList<AdminLeadChecklistItemViewModel> CreateChecklist(
            AdminLeadDetailInputModel values)
        {
            return new[]
            {
                new AdminLeadChecklistItemViewModel
                {
                    Label = "Recapito presente",
                    IsDone = !string.IsNullOrWhiteSpace(values.Phone) || !string.IsNullOrWhiteSpace(values.Email)
                },
                new AdminLeadChecklistItemViewModel
                {
                    Label = "Zona o immobile",
                    IsDone = !string.IsNullOrWhiteSpace(values.DesiredLocation) || !string.IsNullOrWhiteSpace(values.LinkedPropertyReference)
                },
                new AdminLeadChecklistItemViewModel
                {
                    Label = "Motivo richiesta",
                    IsDone = !string.IsNullOrWhiteSpace(values.ContactReason)
                },
                new AdminLeadChecklistItemViewModel
                {
                    Label = "Referente assegnato",
                    IsDone = values.AssignedAgencyUserId.HasValue
                }
            };
        }

        private static IReadOnlyList<AdminLeadOptionViewModel> CreateSourceOptions(string selected)
        {
            return new[]
            {
                CreateOption("Telefono", "Telefono", selected),
                CreateOption("Ufficio", "Ufficio", selected),
                CreateOption("WhatsApp", "WhatsApp", selected),
                CreateOption("Email", "Email", selected)
            };
        }

        private static IReadOnlyList<AdminLeadOptionViewModel> CreateAdvisorOptions(
            IReadOnlyList<AdminLeadAdvisorDto> advisors,
            Guid? selected)
        {
            var selectedValue = selected?.ToString();
            var orderedAdvisors = advisors
                .OrderBy(advisor => advisor.DisplayName);

            return new[] { CreateOption(null, "Non assegnato", selectedValue) }
                .Concat(orderedAdvisors.Select(advisor =>
                    CreateOption(advisor.Id.ToString(), FirstName(advisor.DisplayName), selectedValue)))
                .ToArray();
        }

        private static IReadOnlyList<AdminLeadOptionViewModel> CreateRequestTypeOptions(string selected)
        {
            return new[]
            {
                CreateOption("Valuation", "Valutazione immobile", selected),
                CreateOption("Buy", "Compra casa", selected),
                CreateOption("Rent", "Affitta casa", selected),
                CreateOption("Sell", "Vende casa", selected)
            };
        }

        private static IReadOnlyList<AdminLeadOptionViewModel> CreateUrgencyOptions(string selected)
        {
            return new[]
            {
                CreateOption("Da capire", "Da capire", selected),
                CreateOption("Alta", "Alta", selected),
                CreateOption("Media", "Media", selected),
                CreateOption("Bassa", "Bassa", selected)
            };
        }

        private static AdminLeadOptionViewModel CreateOption(
            string value,
            string label,
            string selected)
        {
            return new AdminLeadOptionViewModel
            {
                Value = value,
                Label = label,
                IsSelected = value == selected ||
                    (string.IsNullOrWhiteSpace(value) && string.IsNullOrWhiteSpace(selected))
            };
        }

        private static IReadOnlyList<AdminLeadInteractionViewModel> CreateInteractions(
            IReadOnlyList<AdminLeadInteractionDto> interactions)
        {
            var referenceDate = DateTime.UtcNow.Date;
            return interactions
                .Select(interaction => CreateInteraction(interaction, referenceDate))
                .ToArray();
        }

        private static AdminLeadInteractionViewModel CreateInteraction(
            AdminLeadInteractionDto interaction,
            DateTime referenceDate)
        {
            return new AdminLeadInteractionViewModel
            {
                TimeLabel = TimeLabel(interaction.OccurredAtUtc, referenceDate),
                Title = interaction.Channel,
                Notes = interaction.Notes
            };
        }

        private static string TimeLabel(DateTime value, DateTime referenceDate)
        {
            if (value.Date == referenceDate)
            {
                return $"Oggi {value:HH:mm}";
            }

            if (value.Date == referenceDate.AddDays(-1))
            {
                return $"Ieri {value:HH:mm}";
            }

            return value.ToString("dd/MM", System.Globalization.CultureInfo.InvariantCulture);
        }

        private static (string FirstName, string LastName) SplitName(string fullName)
        {
            var parts = (fullName ?? string.Empty)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                return (string.Empty, string.Empty);
            }

            if (parts.Length == 1)
            {
                return (parts[0], string.Empty);
            }

            return (parts[0], string.Join(" ", parts.Skip(1)));
        }

        private static string FullName(string firstName, string lastName, string fallback)
        {
            var value = string.Join(
                " ",
                new[] { firstName, lastName }
                    .Where(part => !string.IsNullOrWhiteSpace(part))
                    .Select(part => part.Trim()));

            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }

        private static string StatusLabel(string status)
        {
            return status switch
            {
                "InContact" => "In contatto",
                "Qualified" => "Convertito",
                "Archived" => "Archiviato",
                _ => "Nuovo"
            };
        }

        private static string StatusDescription(string status)
        {
            return status switch
            {
                "InContact" => "Conversazione gia avviata",
                "Qualified" => "Richiesta strutturata presente",
                "Archived" => "Fuori dalla lavorazione attiva",
                _ => "Non ancora qualificato"
            };
        }

        private static string StatusClass(string status)
        {
            return status switch
            {
                "InContact" => "is-contact",
                "Qualified" => "is-success",
                "Archived" => "is-low",
                _ => "is-new"
            };
        }

        private static string RequestTypeShortLabel(string requestType)
        {
            return requestType switch
            {
                "Valuation" => "Valutazione",
                "Rent" => "Affitto",
                "Sell" => "Vendita",
                _ => "Acquisto"
            };
        }

        private static string RequestTypeDescription(string requestType)
        {
            return requestType switch
            {
                "Valuation" => "Immobile da vendere, dati ancora da completare",
                "Rent" => "Casa in affitto, garanzie e tempi da completare",
                "Sell" => "Vendita immobile, indirizzo e condizioni da verificare",
                _ => "Casa da comprare, budget e zona da completare"
            };
        }

        private static string SourceDescription(string source)
        {
            return source == "Telefono"
                ? "Registrato da contatto telefonico"
                : "Registrato dal canale operativo";
        }

        private static string AdvisorSummary(
            IReadOnlyList<AdminLeadAdvisorDto> advisors,
            Guid? assignedAgencyUserId)
        {
            if (!assignedAgencyUserId.HasValue)
            {
                return "Non assegnato";
            }

            var advisor = advisors.FirstOrDefault(item => item.Id == assignedAgencyUserId);
            return advisor == null ? "Non assegnato" : FirstName(advisor.DisplayName);
        }

        private static string FirstName(string displayName)
        {
            return displayName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault() ?? displayName;
        }

        private static string PhoneHref(string phone)
        {
            var digits = Digits(phone);
            return string.IsNullOrWhiteSpace(digits) ? null : $"tel:{digits}";
        }

        private static string WhatsAppHref(string phone)
        {
            var digits = Digits(phone);
            if (string.IsNullOrWhiteSpace(digits))
            {
                return null;
            }

            var normalized = digits.StartsWith("0039", StringComparison.Ordinal)
                ? digits.Substring(2)
                : digits;

            normalized = normalized.StartsWith("39", StringComparison.Ordinal)
                ? normalized
                : $"39{normalized}";

            return $"https://wa.me/{normalized}";
        }

        private static string Digits(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var digits = new string(value.Where(char.IsDigit).ToArray());
            return string.IsNullOrWhiteSpace(digits) ? null : digits;
        }
    }
}
