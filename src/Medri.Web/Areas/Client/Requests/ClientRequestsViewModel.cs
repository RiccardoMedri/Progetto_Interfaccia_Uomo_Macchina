using System;
using System.Collections.Generic;
using Medri.Web.Areas.Client;

namespace Medri.Web.Areas.Client.Requests
{
    public sealed class ClientRequestsInputModel
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }

    public sealed class ClientRequestsViewModel
    {
        public ClientAreaNavigationViewModel Navigation { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalItems { get; set; }

        public int TotalPages { get; set; } = 1;

        public IReadOnlyList<ClientRequestPageSizeOptionViewModel> PageSizeOptions { get; set; } =
            Array.Empty<ClientRequestPageSizeOptionViewModel>();

        public IReadOnlyList<ClientRequestCardViewModel> Requests { get; set; } =
            Array.Empty<ClientRequestCardViewModel>();

        public bool HasRequests => Requests.Count > 0;

        public bool HasPagination => TotalPages > 1;

        public bool HasPreviousPage => Page > 1;

        public bool HasNextPage => Page < TotalPages;
    }

    public sealed class ClientRequestCardViewModel
    {
        public string StatusLabel { get; set; }

        public bool IsStrongStatus { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }
    }

    public sealed class ClientRequestPageSizeOptionViewModel
    {
        public int Value { get; set; }

        public string Label => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

        public bool IsSelected { get; set; }
    }
}
