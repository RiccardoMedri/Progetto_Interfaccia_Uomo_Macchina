using System;
using System.Collections.Generic;
using Medri.Web.Areas.Client;

namespace Medri.Web.Areas.Client.Saved
{
    public sealed class ClientSavedViewModel
    {
        public ClientAreaNavigationViewModel Navigation { get; set; }

        public IReadOnlyList<ClientSavedPropertyViewModel> Properties { get; set; } =
            Array.Empty<ClientSavedPropertyViewModel>();

        public bool HasProperties => Properties.Count > 0;
    }

    public sealed class ClientSavedPropertyViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Contract { get; set; }

        public string PriceLabel { get; set; }

        public string DisplayLocation { get; set; }

        public string ImageUrl { get; set; }

        public string Slug { get; set; }

        public string BedroomsLabel { get; set; }

        public string BathroomsLabel { get; set; }

        public string SurfaceLabel { get; set; }

        public string FourthFactLabel { get; set; }

        public string SavedNote { get; set; }
    }
}
