using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Medri.Services.Medri.Application;

namespace Medri.Web.Areas.Client.Searches
{
    public sealed class ClientSearchesViewModel
    {
        public ClientAreaNavigationViewModel Navigation { get; set; }

        public IReadOnlyList<ClientSavedSearchItemViewModel> Searches { get; set; } =
            new List<ClientSavedSearchItemViewModel>();

        public bool IsJustSaved { get; set; }

        public bool HasSearches => Searches.Count > 0;
    }

    public sealed class ClientSavedSearchItemViewModel
    {
        public Guid Id { get; set; }

        public string Label { get; set; }

        public string ResultsUrl { get; set; }
    }

    internal static class ClientSearchesViewModelMapper
    {
        public static ClientSearchesViewModel Create(
            IReadOnlyList<ClientSavedSearchDto> savedSearches,
            int favoriteCount,
            int requestCount,
            string searchBaseUrl,
            bool isJustSaved)
        {
            return new ClientSearchesViewModel
            {
                Navigation = ClientAreaNavigationViewModel.Create(
                    ClientAreaTabs.Searches,
                    favoriteCount,
                    requestCount),
                IsJustSaved = isJustSaved,
                Searches = savedSearches.Select(saved => new ClientSavedSearchItemViewModel
                {
                    Id = saved.Id,
                    Label = string.IsNullOrWhiteSpace(saved.Label) ? "Tutti gli immobili" : saved.Label,
                    ResultsUrl = BuildResultsUrl(searchBaseUrl, saved.QueryString)
                }).ToArray()
            };
        }

        private static string BuildResultsUrl(string searchBaseUrl, string queryString)
        {
            var baseUrl = string.IsNullOrWhiteSpace(searchBaseUrl)
                ? "/immobili"
                : searchBaseUrl;

            if (string.IsNullOrWhiteSpace(queryString))
            {
                return baseUrl;
            }

            return baseUrl + QueryString.Create(QueryHelpers.ParseQuery(queryString)).ToUriComponent();
        }
    }
}
