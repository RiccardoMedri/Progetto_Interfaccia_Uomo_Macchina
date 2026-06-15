using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Medri.Services.Medri.Application;
using Medri.Web.Features.Property;
using Medri.Web.Features.Search;
using Medri.Web.Infrastructure;

namespace Medri.Web.Features.Home
{
    public partial class HomeController : Controller
    {
        private readonly IGoogleReviewsService googleReviewsService;
        private readonly HomeFeaturedListingsQuery homeFeaturedListingsQuery;

        public HomeController(
            IGoogleReviewsService googleReviewsService,
            HomeFeaturedListingsQuery homeFeaturedListingsQuery)
        {
            this.googleReviewsService = googleReviewsService;
            this.homeFeaturedListingsQuery = homeFeaturedListingsQuery;
        }

        [HttpGet]
        public virtual async Task<IActionResult> Index()
        {
            var featuredListings = await homeFeaturedListingsQuery.ExecuteAsync(
                AuthenticatedUserId.Get(User),
                HttpContext.RequestAborted);
            var favoriteReturnUrl = HttpContext.Request.Path.Value + HttpContext.Request.QueryString.Value;

            var viewModel = new HomeIndexViewModel
            {
                Reviews = await googleReviewsService.GetHomeReviewsAsync(HttpContext.RequestAborted),
                FeaturedListings = featuredListings
                    .Select((listing, index) => new HomeFeaturedListingCardViewModel
                    {
                        Property = new SearchResultCardViewModel
                        {
                            Id = listing.Id.ToString(),
                            Title = listing.Title,
                            Slug = listing.Slug,
                            Tag = listing.Status,
                            PriceLabel = PropertyFormatting.FormatPrice(listing.Price, listing.Contract),
                            DisplayLocation = PropertyFormatting.DisplayOrFallback(listing.DisplayLocation, listing.Location),
                            FactsLabel = $"{listing.Rooms} locali - {listing.Bathrooms} bagni - {listing.SurfaceSquareMeters} mq",
                            ImageUrl = listing.ImageUrl,
                            IsSaved = listing.IsSaved
                        },
                        FavoriteReturnUrl = favoriteReturnUrl,
                        IsPrimary = index == 0
                    })
                    .ToList()
            };

            return View(viewModel);
        }
    }
}
