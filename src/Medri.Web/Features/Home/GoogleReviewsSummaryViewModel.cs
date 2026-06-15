using System.Collections.Generic;
using System.Linq;

namespace Medri.Web.Features.Home
{
    public class GoogleReviewsSummaryViewModel
    {
        private const int VisibleReviewCardCount = 5;

        public IReadOnlyList<GoogleReviewViewModel> Reviews { get; set; } = new List<GoogleReviewViewModel>();

        public IReadOnlyList<GoogleReviewViewModel> VisibleReviews
        {
            get
            {
                var reviews = Reviews?.Take(VisibleReviewCardCount).ToList() ?? new List<GoogleReviewViewModel>();
                if (reviews.Count == 0)
                {
                    while (reviews.Count < VisibleReviewCardCount)
                    {
                        reviews.Add(GoogleReviewViewModel.Unavailable());
                    }
                }

                return reviews;
            }
        }

        public IReadOnlyList<GoogleReviewViewModel> CarouselReviews
        {
            get
            {
                var visible = VisibleReviews.ToList();
                return visible.Concat(visible).ToList();
            }
        }

        public static GoogleReviewsSummaryViewModel Unconfigured()
        {
            return new GoogleReviewsSummaryViewModel();
        }

        public static GoogleReviewsSummaryViewModel Unavailable()
        {
            return new GoogleReviewsSummaryViewModel();
        }
    }
}
