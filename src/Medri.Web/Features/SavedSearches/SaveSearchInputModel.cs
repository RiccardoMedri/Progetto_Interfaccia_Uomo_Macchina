using System.Collections.Generic;

namespace Medri.Web.Features.SavedSearches
{
    public class SaveSearchInputModel
    {
        public List<string> Contracts { get; set; } = new List<string>();

        public List<string> PropertyTypes { get; set; } = new List<string>();

        public List<string> Rooms { get; set; } = new List<string>();

        public List<string> Zones { get; set; } = new List<string>();

        public List<string> PriceRanges { get; set; } = new List<string>();

        public List<string> Features { get; set; } = new List<string>();

        public List<string> Bathrooms { get; set; } = new List<string>();

        public List<string> SurfaceRanges { get; set; } = new List<string>();

        public List<string> EnergyClasses { get; set; } = new List<string>();

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string View { get; set; }

        public string Sort { get; set; }

        public string ReturnUrl { get; set; }
    }
}
