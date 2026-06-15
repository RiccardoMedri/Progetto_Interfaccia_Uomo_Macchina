using System;
using System.Linq;
using Medri.Services.Medri.Application;
using Medri.Web.Areas.Client;

namespace Medri.Web.Areas.Client.Requests
{
    internal static class ClientRequestsViewModelMapper
    {
        public static ClientRequestsViewModel Create(ClientRequestsResultDto result)
        {
            return new ClientRequestsViewModel
            {
                Navigation = ClientAreaNavigationViewModel.Create(
                    ClientAreaTabs.Requests,
                    result.FavoriteCount,
                    result.TotalItems),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems,
                TotalPages = TotalPages(result.TotalItems, result.PageSize),
                PageSizeOptions = new[] { 10, 20, 25 }
                    .Select(value => new ClientRequestPageSizeOptionViewModel
                    {
                        Value = value,
                        IsSelected = value == result.PageSize
                    })
                    .ToArray(),
                Requests = result.Requests.Select(CreateCard).ToArray()
            };
        }

        private static ClientRequestCardViewModel CreateCard(ClientRequestDto request)
        {
            return new ClientRequestCardViewModel
            {
                StatusLabel = StatusLabel(request.Status),
                IsStrongStatus = request.Status == "InMatching",
                Title = Title(request),
                Summary = Summary(request, includeMinimumQualifier: true)
            };
        }

        private static string StatusLabel(string status)
        {
            return status == "InMatching"
                ? "In matching"
                : status == "Scheduled"
                    ? "Appuntamento fissato"
                    : "Ricevuta";
        }

        private static string Title(ClientRequestDto request)
        {
            return request.RequestType switch
            {
                "Buy" => "Sto cercando casa da acquistare",
                "Rent" => "Sto cercando casa in affitto",
                "Valuation" => "Vorrei valutare il mio immobile",
                "Sell" => "Vorrei vendere il mio immobile",
                "RentOut" => "Vorrei mettere in affitto il mio immobile",
                "PropertyContact" when !string.IsNullOrWhiteSpace(request.PropertyTitle) =>
                    "Informazioni su " + request.PropertyTitle,
                _ => "Richiesta immobiliare"
            };
        }

        private static string Summary(ClientRequestDto request, bool includeMinimumQualifier)
        {
            if (request.RequestType == "Buy")
            {
                var buyLocation = string.IsNullOrWhiteSpace(request.DesiredLocation)
                    ? "Zona da definire"
                    : request.DesiredLocation;
                var rooms = request.MinimumRooms.HasValue
                    ? $"{(includeMinimumQualifier ? "almeno " : string.Empty)}{request.MinimumRooms.Value} camere"
                    : "caratteristiche da definire";
                var budget = string.IsNullOrWhiteSpace(request.SustainableBudgetLabel)
                    ? "budget da definire"
                    : request.SustainableBudgetLabel;
                return $"{buyLocation}, {rooms}, {budget}.";
            }

            if (request.RequestType == "Valuation")
            {
                var propertyType = string.IsNullOrWhiteSpace(request.PropertyType)
                    ? "Immobile"
                    : request.PropertyType;
                return $"{propertyType} a {request.DesiredLocation}, richiesta inviata per una prima valutazione.";
            }

            if (request.RequestType == "PropertyContact" &&
                !string.IsNullOrWhiteSpace(request.Notes))
            {
                return request.Notes;
            }

            var location = string.IsNullOrWhiteSpace(request.DesiredLocation)
                ? "Zona da definire"
                : request.DesiredLocation;
            return $"{location}, richiesta inviata all'agenzia.";
        }

        private static int TotalPages(int totalItems, int pageSize)
        {
            return Math.Max(1, (int)Math.Ceiling(totalItems / (double)Math.Max(1, pageSize)));
        }
    }
}
