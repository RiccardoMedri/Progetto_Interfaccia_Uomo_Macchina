using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Medri.Services.Medri.Application;
using Medri.Web.Areas.Admin;

namespace Medri.Web.Areas.Admin.Properties
{
    internal static class AdminPropertyListViewModelMapper
    {
        public static AdminPropertyListViewModel Create(
            AdminPropertyListResultDto result,
            AdminPropertyListInputModel input,
            ClaimsPrincipal user)
        {
            input ??= new AdminPropertyListInputModel();

            return new AdminPropertyListViewModel
            {
                Navigation = AdminNavigationViewModelMapper.Create(
                    user,
                    result.LeadCount,
                    result.ActiveRequestCount,
                    result.ListingCount,
                    AdminSections.Properties),
                SearchTerm = input.SearchTerm,
                Status = AdminPropertyListInputModel.JoinValues(input.SelectedStatuses()),
                Contract = AdminPropertyListInputModel.JoinValues(input.SelectedContracts()),
                Advisor = AdminPropertyListInputModel.JoinValues(input.SelectedAdvisors()),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems,
                TotalPages = Math.Max(1, (int)Math.Ceiling(result.TotalItems / (double)Math.Max(1, result.PageSize))),
                PageSizeOptions = new[] { 15, 25, 50 },
                StatusCards = CreateStatusCards(result, input.SelectedStatuses()),
                StatusOptions = CreateStatusOptions(input.SelectedStatuses()),
                ContractOptions = CreateContractOptions(input.SelectedContracts()),
                AdvisorOptions = CreateAdvisorOptions(result.Advisors, input.SelectedAdvisors()),
                Properties = result.Properties
                    .Select(CreateRow)
                    .ToArray(),
                FeaturedSlots = result.FeaturedSlots
                    .Select(slot => new AdminPropertyFeaturedSlotViewModel
                    {
                        SlotNumber = slot.SlotNumber,
                        Reference = slot.Reference,
                        Title = slot.Title,
                        DisplayLocation = slot.DisplayLocation
                    })
                    .ToArray()
            };
        }

        private static IReadOnlyList<AdminPropertyStatusCardViewModel> CreateStatusCards(
            AdminPropertyListResultDto result,
            IReadOnlyList<string> selectedStatuses)
        {
            return new[]
            {
                new AdminPropertyStatusCardViewModel
                {
                    Status = "Incomplete",
                    Label = "Incompleti",
                    Count = result.IncompleteCount,
                    Description = "Mancano dati minimi, media o testi necessari alla scheda pubblica.",
                    IsSelected = selectedStatuses.Contains("Incomplete"),
                    ToggledStatusFilter = ToggleStatus(selectedStatuses, "Incomplete")
                },
                new AdminPropertyStatusCardViewModel
                {
                    Status = "Ready",
                    Label = "Pronti",
                    Count = result.ReadyCount,
                    Description = "Annunci completi e pubblicabili sul sito.",
                    IsSelected = selectedStatuses.Contains("Ready"),
                    ToggledStatusFilter = ToggleStatus(selectedStatuses, "Ready")
                },
                new AdminPropertyStatusCardViewModel
                {
                    Status = "Published",
                    Label = "Pubblicati",
                    Count = result.PublishedCount,
                    Description = "Annunci gia visibili agli utenti esterni.",
                    IsSelected = selectedStatuses.Contains("Published"),
                    ToggledStatusFilter = ToggleStatus(selectedStatuses, "Published")
                },
                new AdminPropertyStatusCardViewModel
                {
                    Status = "NeedsUpdate",
                    Label = "Da aggiornare",
                    Count = result.NeedsUpdateCount,
                    Description = "Annunci online con prezzo, disponibilita o contenuti da rivedere.",
                    IsSelected = selectedStatuses.Contains("NeedsUpdate"),
                    ToggledStatusFilter = ToggleStatus(selectedStatuses, "NeedsUpdate")
                }
            };
        }

        private static IReadOnlyList<AdminPropertyOptionViewModel> CreateContractOptions(
            IReadOnlyList<string> selected)
        {
            return new[]
            {
                CreateOption(null, "Tutti", selected),
                CreateOption("Vendita", "Vendita", selected),
                CreateOption("Affitto", "Affitto", selected)
            };
        }

        private static IReadOnlyList<AdminPropertyOptionViewModel> CreateStatusOptions(
            IReadOnlyList<string> selected)
        {
            return new[]
            {
                CreateOption("Incomplete", "Incompleti", selected),
                CreateOption("Ready", "Pronti", selected),
                CreateOption("Published", "Pubblicati", selected),
                CreateOption("NeedsUpdate", "Da aggiornare", selected)
            };
        }

        private static IReadOnlyList<AdminPropertyOptionViewModel> CreateAdvisorOptions(
            IReadOnlyList<AdminPropertyAdvisorDto> advisors,
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

        private static AdminPropertyOptionViewModel CreateOption(
            string value,
            string label,
            IReadOnlyList<string> selected)
        {
            return new AdminPropertyOptionViewModel
            {
                Value = value,
                Label = label,
                IsSelected = string.IsNullOrWhiteSpace(value)
                    ? selected.Count == 0
                    : selected.Contains(value)
            };
        }

        private static AdminPropertyRowViewModel CreateRow(AdminPropertyRowDto property)
        {
            return new AdminPropertyRowViewModel
            {
                Reference = property.Reference,
                Title = property.Title,
                DisplayLocation = property.DisplayLocation,
                ThumbnailUrl = ThumbnailUrl(property.ImageUrl),
                ContractLabel = property.Contract,
                StatusLabel = StatusLabel(property.PublicationStatus),
                StatusClass = StatusClass(property.PublicationStatus),
                CompletionPercent = property.CompletionPercent,
                MissingItems = MissingItems(property.MissingItems),
                AdvisorLabel = string.IsNullOrWhiteSpace(property.AdvisorDisplayName)
                    ? "Non assegnato"
                    : FirstName(property.AdvisorDisplayName),
                IsFeatured = property.IsFeatured,
                CanFeature = property.CanFeature,
                CanArchive = property.PublicationStatus != "Archived"
            };
        }

        private static string ToggleStatus(
            IReadOnlyList<string> selectedStatuses,
            string status)
        {
            var selected = selectedStatuses ?? Array.Empty<string>();
            var values = selected.Contains(status)
                ? selected.Where(value => value != status)
                : selected.Concat(new[] { status });

            return AdminPropertyListInputModel.JoinValues(values);
        }

        private static string MissingItems(string value)
        {
            return string.IsNullOrWhiteSpace(value) ||
                value == "Nessun dato mancante"
                    ? "-"
                    : value;
        }

        private static string ThumbnailUrl(string imageUrl)
        {
            const string ReferenceAssetPrefix = "/medri-reference/assets/properties/";
            const string ThumbnailPrefix = "/medri-reference/assets/properties/thumbs/";

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return null;
            }

            if (!imageUrl.StartsWith(ReferenceAssetPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return imageUrl;
            }

            var fileName = imageUrl
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault();

            return string.IsNullOrWhiteSpace(fileName)
                ? imageUrl
                : $"{ThumbnailPrefix}{fileName}";
        }

        private static string StatusLabel(string status)
        {
            return status switch
            {
                "Incomplete" => "Incompleto",
                "Ready" => "Pronto",
                "Published" => "Pubblicato",
                "NeedsUpdate" => "Da aggiornare",
                _ => "Riservato"
            };
        }

        private static string StatusClass(string status)
        {
            return status switch
            {
                "Incomplete" => "is-incomplete",
                "Ready" => "is-success",
                "Published" => "is-active",
                "NeedsUpdate" => "is-update-needed",
                _ => "is-low"
            };
        }

        private static string FirstName(string displayName)
        {
            return displayName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault() ?? displayName;
        }
    }
}
