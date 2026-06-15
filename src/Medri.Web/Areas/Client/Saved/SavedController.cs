using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Medri.Services.Medri.Application;
using Medri.Web.Infrastructure;

namespace Medri.Web.Areas.Client.Saved
{
    [Area("Client")]
    public partial class SavedController : ClientAreaController
    {
        private readonly GetFavoritePropertiesQuery getFavoritePropertiesQuery;
        private readonly ClientRequestCountQuery clientRequestCountQuery;

        public SavedController(
            GetFavoritePropertiesQuery getFavoritePropertiesQuery,
            ClientRequestCountQuery clientRequestCountQuery)
        {
            this.getFavoritePropertiesQuery = getFavoritePropertiesQuery;
            this.clientRequestCountQuery = clientRequestCountQuery;
        }

        [HttpGet]
        public virtual async Task<IActionResult> Index()
        {
            var userId = AuthenticatedUserId.Get(User);
            if (!userId.HasValue)
            {
                return Challenge();
            }

            var properties = await getFavoritePropertiesQuery.ExecuteAsync(
                userId.Value,
                HttpContext.RequestAborted);
            var requestCount = await clientRequestCountQuery.ExecuteAsync(
                userId.Value,
                HttpContext.RequestAborted);

            return View(ClientSavedViewModelMapper.Create(properties, requestCount));
        }
    }
}
