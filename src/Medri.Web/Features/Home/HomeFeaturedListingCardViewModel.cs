using Medri.Web.Features.Search;

namespace Medri.Web.Features.Home
{
    public class HomeFeaturedListingCardViewModel
    {
        public SearchResultCardViewModel Property { get; set; }

        public string FavoriteReturnUrl { get; set; }

        public bool IsPrimary { get; set; }
    }
}
