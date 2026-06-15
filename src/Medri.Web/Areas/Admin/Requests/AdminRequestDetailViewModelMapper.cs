using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using Medri.Services.Medri.Application;
using Medri.Web.Areas.Admin;
using Medri.Web.Areas.Admin.Leads;

namespace Medri.Web.Areas.Admin.Requests
{
    internal static class AdminRequestDetailViewModelMapper
    {
        public static AdminRequestDetailViewModel Create(
            AdminRequestDetailResultDto result,
            AdminRequestDetailInputModel input,
            ClaimsPrincipal user)
        {
            var values = input ?? CreateInput(result);
            var model = new AdminRequestDetailViewModel
            {
                Navigation = AdminNavigationViewModelMapper.Create(
                    user,
                    result.LeadCount,
                    result.ActiveRequestCount,
                    result.ListingCount,
                    AdminSections.Requests),
                Reference = result.Reference,
                IsCreateMode = result.IsCreateMode,
                DisplayName = FullName(values.FirstName, values.LastName, result.FullName),
                StatusLabel = StatusLabel(result.Status),
                StatusClass = StatusClass(result.Status),
                PriorityLabel = PriorityLabel(result.Priority),
                LinkedLeadReference = result.LinkedLeadReference,
                FirstName = values.FirstName,
                LastName = values.LastName,
                Phone = values.Phone,
                Email = values.Email,
                SourceChannel = values.SourceChannel,
                Status = values.Status,
                AssignedAgencyUserId = values.AssignedAgencyUserId,
                RequestType = values.RequestType,
                MaximumBudgetLabel = values.MaximumBudgetLabel,
                PreferredBudgetLabel = values.PreferredBudgetLabel,
                DesiredLocation = values.DesiredLocation,
                AcceptableLocations = values.AcceptableLocations,
                MinimumRooms = values.MinimumRooms,
                AccessibilityConstraint = values.AccessibilityConstraint,
                TimeFrame = values.TimeFrame,
                FinancingStatus = values.FinancingStatus,
                PropertyToSellStatus = values.PropertyToSellStatus,
                SummaryNotes = values.SummaryNotes,
                NeedsAfterContact = values.NeedsAfterContact,
                DesiredPreferenceTagsText = values.DesiredPreferenceTagsText,
                NegotiablePreferenceTagsText = values.NegotiablePreferenceTagsText,
                PhoneHref = PhoneHref(values.Phone),
                WhatsAppHref = WhatsAppHref(values.Phone),
                CanArchive = !result.IsCreateMode && result.Status != "Archived",
                SourceOptions = CreateSourceOptions(values.SourceChannel),
                AdvisorOptions = CreateAdvisorOptions(result.Advisors, values.AssignedAgencyUserId),
                StatusOptions = CreateStatusOptions(values.Status),
                RequestTypeOptions = CreateRequestTypeOptions(values.RequestType),
                TimeFrameOptions = CreateTimeFrameOptions(values.TimeFrame),
                FinancingOptions = CreateFinancingOptions(values.FinancingStatus),
                PropertyToSellOptions = CreatePropertyToSellOptions(values.PropertyToSellStatus),
                ConstraintCards = CreateConstraintCards(values),
                DesiredPreferenceChips = SplitTags(values.DesiredPreferenceTagsText),
                NegotiablePreferenceChips = SplitTags(values.NegotiablePreferenceTagsText),
                Interactions = CreateInteractions(result.Interactions),
                QualificationChecklist = CreateChecklist(values, result)
            };

            model.SummaryCards = CreateSummaryCards(model, result);
            return model;
        }

        private static AdminRequestDetailInputModel CreateInput(AdminRequestDetailResultDto result)
        {
            if (result.IsCreateMode)
            {
                return new AdminRequestDetailInputModel
                {
                    SourceChannel = result.SourceChannel,
                    Status = result.Status,
                    RequestType = result.RequestType
                };
            }

            var name = SplitName(result.FullName);

            return new AdminRequestDetailInputModel
            {
                FirstName = name.FirstName,
                LastName = name.LastName,
                Phone = result.Phone,
                Email = result.Email,
                SourceChannel = result.SourceChannel,
                Status = result.Status,
                AssignedAgencyUserId = result.AssignedAgencyUserId,
                RequestType = result.RequestType,
                MaximumBudgetLabel = BudgetMaximumLabel(result),
                PreferredBudgetLabel = result.PreferredBudgetLabel,
                DesiredLocation = result.DesiredLocation,
                AcceptableLocations = result.AcceptableLocations,
                MinimumRooms = result.MinimumRooms,
                AccessibilityConstraint = result.AccessibilityConstraint,
                TimeFrame = result.TimeFrame,
                FinancingStatus = result.FinancingStatus,
                PropertyToSellStatus = result.PropertyToSellStatus,
                SummaryNotes = result.SummaryNotes,
                NeedsAfterContact = result.NeedsAfterContact,
                DesiredPreferenceTagsText = TagsToTextarea(result.DesiredPreferenceTags),
                NegotiablePreferenceTagsText = TagsToTextarea(result.NegotiablePreferenceTags)
            };
        }

        private static IReadOnlyList<AdminRequestSummaryCardViewModel> CreateSummaryCards(
            AdminRequestDetailViewModel model,
            AdminRequestDetailResultDto result)
        {
            return new[]
            {
                new AdminRequestSummaryCardViewModel
                {
                    Label = "Tipo",
                    Value = RequestTypeSummary(model.RequestType),
                    Description = string.IsNullOrWhiteSpace(model.SummaryNotes)
                        ? RequestTypeDescription(model.RequestType)
                        : "Note operative presenti"
                },
                new AdminRequestSummaryCardViewModel
                {
                    Label = "Stato",
                    Value = model.StatusLabel,
                    Description = StatusDescription(result.Status)
                },
                new AdminRequestSummaryCardViewModel
                {
                    Label = "Priorita",
                    Value = model.PriorityLabel,
                    Description = string.IsNullOrWhiteSpace(model.TimeFrame)
                        ? "Tempistiche da confermare"
                        : $"Decisione prevista {model.TimeFrame.ToLowerInvariant()}"
                },
                new AdminRequestSummaryCardViewModel
                {
                    Label = "Referente",
                    Value = AdvisorSummary(result.Advisors, model.AssignedAgencyUserId),
                    Description = model.AssignedAgencyUserId.HasValue
                        ? "Assegnata dopo conversione lead"
                        : "Da assegnare"
                }
            };
        }

        private static IReadOnlyList<AdminRequestInfoCardViewModel> CreateConstraintCards(
            AdminRequestDetailInputModel values)
        {
            return new[]
            {
                new AdminRequestInfoCardViewModel
                {
                    Label = "Budget",
                    Value = string.IsNullOrWhiteSpace(values.MaximumBudgetLabel)
                        ? "Da confermare"
                        : $"Non oltre {values.MaximumBudgetLabel}",
                    Description = BudgetDescription(values)
                },
                new AdminRequestInfoCardViewModel
                {
                    Label = "Locali",
                    Value = values.MinimumRooms.HasValue
                        ? $"Almeno {values.MinimumRooms.Value} camere"
                        : "Da confermare",
                    Description = RoomsDescription(values)
                },
                new AdminRequestInfoCardViewModel
                {
                    Label = "Accessibilita",
                    Value = string.IsNullOrWhiteSpace(values.AccessibilityConstraint)
                        ? "Da confermare"
                        : values.AccessibilityConstraint,
                    Description = string.IsNullOrWhiteSpace(values.AccessibilityConstraint)
                        ? "Vincoli fisici o familiari non ancora indicati"
                        : "Vincolo indicato nella richiesta"
                },
                new AdminRequestInfoCardViewModel
                {
                    Label = "Zona",
                    Value = LocationValue(values),
                    Description = LocationDescription(values)
                }
            };
        }

        private static IReadOnlyList<AdminLeadChecklistItemViewModel> CreateChecklist(
            AdminRequestDetailInputModel values,
            AdminRequestDetailResultDto result)
        {
            return new[]
            {
                new AdminLeadChecklistItemViewModel
                {
                    Label = "Recapito verificato",
                    IsDone = !string.IsNullOrWhiteSpace(values.Phone) || !string.IsNullOrWhiteSpace(values.Email)
                },
                new AdminLeadChecklistItemViewModel
                {
                    Label = "Budget presente",
                    IsDone = !string.IsNullOrWhiteSpace(values.MaximumBudgetLabel) || !string.IsNullOrWhiteSpace(values.PreferredBudgetLabel)
                },
                new AdminLeadChecklistItemViewModel
                {
                    Label = "Zona ideale definita",
                    IsDone = !string.IsNullOrWhiteSpace(values.DesiredLocation)
                },
                new AdminLeadChecklistItemViewModel
                {
                    Label = "Vincoli minimi chiari",
                    IsDone = values.MinimumRooms.HasValue && !string.IsNullOrWhiteSpace(values.MaximumBudgetLabel)
                },
                new AdminLeadChecklistItemViewModel
                {
                    Label = "Mutuo da confermare",
                    IsDone = values.FinancingStatus == "Gia verificato"
                }
            };
        }

        private static IReadOnlyList<AdminLeadOptionViewModel> CreateSourceOptions(string selected)
        {
            return DistinctOptions(new[]
            {
                CreateOption("Lead convertito", "Lead convertito", selected),
                CreateOption("Form sito", "Form sito", selected),
                CreateOption("Telefono", "Telefono", selected),
                CreateOption("Ufficio", "Ufficio", selected),
                CreateOption("WhatsApp", "WhatsApp", selected),
                CreateOption("Email", "Email", selected),
                CreateOption(selected, selected, selected)
            });
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
                CreateOption("Buy", "Comprare casa", selected),
                CreateOption("Rent", "Affittare casa", selected),
                CreateOption("Sell", "Vendere casa", selected),
                CreateOption("Valuation", "Valutare immobile", selected)
            };
        }

        private static IReadOnlyList<AdminLeadOptionViewModel> CreateStatusOptions(string selected)
        {
            return new[]
            {
                CreateOption("New", "Nuova", selected),
                CreateOption("Updating", "In aggiornamento", selected),
                CreateOption("InMatching", "In matching", selected),
                CreateOption("Archived", "Archiviata", selected)
            };
        }

        private static IReadOnlyList<AdminLeadOptionViewModel> CreateTimeFrameOptions(string selected)
        {
            return new[]
            {
                CreateOption("Entro 3 mesi", "Entro 3 mesi", selected),
                CreateOption("Entro 6 mesi", "Entro 6 mesi", selected),
                CreateOption("Non urgente", "Non urgente", selected)
            };
        }

        private static IReadOnlyList<AdminLeadOptionViewModel> CreateFinancingOptions(string selected)
        {
            return new[]
            {
                CreateOption("Pre-delibera in corso", "Pre-delibera in corso", selected),
                CreateOption("Gia verificato", "Gia verificato", selected),
                CreateOption("Da verificare", "Da verificare", selected)
            };
        }

        private static IReadOnlyList<AdminLeadOptionViewModel> CreatePropertyToSellOptions(string selected)
        {
            return new[]
            {
                CreateOption("No", "No", selected),
                CreateOption("Si", "Si", selected),
                CreateOption("Da capire", "Da capire", selected)
            };
        }

        private static IReadOnlyList<AdminLeadInteractionViewModel> CreateInteractions(
            IReadOnlyList<AdminLeadInteractionDto> interactions)
        {
            var referenceDate = DateTime.UtcNow.Date;

            return interactions
                .Select(interaction => new AdminLeadInteractionViewModel
                {
                    TimeLabel = TimeLabel(interaction.OccurredAtUtc, referenceDate),
                    Title = interaction.Channel,
                    Notes = interaction.Notes
                })
                .ToArray();
        }

        private static string TimeLabel(DateTime value, DateTime referenceDate)
        {
            if (value.Date == referenceDate)
            {
                return $"Oggi {value.ToString("HH:mm", CultureInfo.InvariantCulture)}";
            }

            if (value.Date == referenceDate.AddDays(-1))
            {
                return $"Ieri {value.ToString("HH:mm", CultureInfo.InvariantCulture)}";
            }

            return value.ToString("dd/MM", CultureInfo.InvariantCulture);
        }

        private static AdminLeadOptionViewModel CreateOption(
            string value,
            string label,
            string selected)
        {
            return new AdminLeadOptionViewModel
            {
                Value = value,
                Label = string.IsNullOrWhiteSpace(label) ? value : label,
                IsSelected = value == selected ||
                    (string.IsNullOrWhiteSpace(value) && string.IsNullOrWhiteSpace(selected))
            };
        }

        private static IReadOnlyList<AdminLeadOptionViewModel> DistinctOptions(
            IEnumerable<AdminLeadOptionViewModel> options)
        {
            return options
                .Where(option => !string.IsNullOrWhiteSpace(option.Value))
                .GroupBy(option => option.Value)
                .Select(group => group.First())
                .ToArray();
        }

        private static string BudgetMaximumLabel(AdminRequestDetailResultDto result)
        {
            if (result.MaximumPrice.HasValue)
            {
                return $"EUR {result.MaximumPrice.Value.ToString("N0", CultureInfo.GetCultureInfo("it-IT"))}";
            }

            if (string.IsNullOrWhiteSpace(result.MaximumBudgetLabel))
            {
                return null;
            }

            return result.MaximumBudgetLabel
                .Replace("max ", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Trim();
        }

        private static IReadOnlyList<string> SplitTags(string value)
        {
            return (value ?? string.Empty)
                .Split(new[] { "|", "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToArray();
        }

        private static string TagsToTextarea(string value)
        {
            var tags = SplitTags(value);
            return tags.Count == 0 ? null : string.Join(Environment.NewLine, tags);
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

        private static string RequestTypeSummary(string requestType)
        {
            return requestType switch
            {
                "Rent" => "Affitta casa",
                "Sell" => "Vende casa",
                "Valuation" => "Valuta immobile",
                _ => "Compra casa"
            };
        }

        private static string RequestTypeDescription(string requestType)
        {
            return requestType switch
            {
                "Rent" => "Ricerca abitazione in affitto",
                "Sell" => "Valutazione e vendita proprietario",
                "Valuation" => "Richiesta valutazione immobile",
                _ => "Ricerca abitazione principale"
            };
        }

        private static string StatusLabel(string status)
        {
            return status switch
            {
                "Updating" => "In aggiornamento",
                "Archived" => "Archiviata",
                "New" => "Nuova",
                _ => "In matching"
            };
        }

        private static string PriorityLabel(string priority)
        {
            return string.IsNullOrWhiteSpace(priority) ? "Da capire" : priority;
        }

        private static string StatusDescription(string status)
        {
            return status switch
            {
                "Updating" => "Preferenze in aggiornamento",
                "Archived" => "Fuori dalla lista attiva",
                "New" => "Primo controllo ancora da completare",
                _ => "Profilo pronto per proposte mirate"
            };
        }

        private static string StatusClass(string status)
        {
            return status switch
            {
                "Updating" => "is-warning",
                "Archived" => "is-low",
                "New" => "is-new",
                _ => "is-matching"
            };
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

        private static string BudgetDescription(AdminRequestDetailInputModel values)
        {
            if (!string.IsNullOrWhiteSpace(values.PreferredBudgetLabel))
            {
                return $"Preferenza indicata: {values.PreferredBudgetLabel}";
            }

            return string.IsNullOrWhiteSpace(values.MaximumBudgetLabel)
                ? "Budget non ancora indicato"
                : "Soglia massima indicata dal cliente";
        }

        private static string RoomsDescription(AdminRequestDetailInputModel values)
        {
            return values.MinimumRooms.HasValue
                ? "Numero minimo indicato dal cliente"
                : "Numero minimo da verificare";
        }

        private static string LocationValue(AdminRequestDetailInputModel values)
        {
            if (!string.IsNullOrWhiteSpace(values.DesiredLocation))
            {
                return values.DesiredLocation;
            }

            if (!string.IsNullOrWhiteSpace(values.AcceptableLocations))
            {
                return values.AcceptableLocations;
            }

            return "Da confermare";
        }

        private static string LocationDescription(AdminRequestDetailInputModel values)
        {
            if (!string.IsNullOrWhiteSpace(values.DesiredLocation) &&
                !string.IsNullOrWhiteSpace(values.AcceptableLocations))
            {
                return $"Alternative: {values.AcceptableLocations}";
            }

            return string.IsNullOrWhiteSpace(values.AcceptableLocations)
                ? "Zone alternative non ancora indicate"
                : "Zone accettabili indicate dal cliente";
        }
    }
}
