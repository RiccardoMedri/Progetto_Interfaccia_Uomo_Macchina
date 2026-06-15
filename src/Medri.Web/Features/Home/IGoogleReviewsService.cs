using System.Threading;
using System.Threading.Tasks;

namespace Medri.Web.Features.Home
{
    public interface IGoogleReviewsService
    {
        Task<GoogleReviewsSummaryViewModel> GetHomeReviewsAsync(CancellationToken cancellationToken);
    }
}
