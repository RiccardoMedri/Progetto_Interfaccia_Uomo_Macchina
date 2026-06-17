using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Medri.Services.Medri.Application;
using Medri.Web.Infrastructure;

namespace Medri.Web.Areas.Client.Searches
{
    [Area("Client")]
    public partial class SearchesController : ClientAreaController
    {
        private readonly ClientSavedSearchesQuery clientSavedSearchesQuery;
        private readonly RemoveClientSavedSearchCommand removeClientSavedSearchCommand;
        private readonly FavoritePropertyCountQuery favoritePropertyCountQuery;
        private readonly ClientRequestCountQuery clientRequestCountQuery;

        public SearchesController(
            ClientSavedSearchesQuery clientSavedSearchesQuery,
            RemoveClientSavedSearchCommand removeClientSavedSearchCommand,
            FavoritePropertyCountQuery favoritePropertyCountQuery,
            ClientRequestCountQuery clientRequestCountQuery)
        {
            this.clientSavedSearchesQuery = clientSavedSearchesQuery;
            this.removeClientSavedSearchCommand = removeClientSavedSearchCommand;
            this.favoritePropertyCountQuery = favoritePropertyCountQuery;
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

            var savedSearches = await clientSavedSearchesQuery.ExecuteAsync(
                userId.Value,
                HttpContext.RequestAborted);
            var favoriteCount = await favoritePropertyCountQuery.ExecuteAsync(
                userId.Value,
                HttpContext.RequestAborted);
            var requestCount = await clientRequestCountQuery.ExecuteAsync(
                userId.Value,
                HttpContext.RequestAborted);

            return View(ClientSearchesViewModelMapper.Create(
                savedSearches,
                favoriteCount,
                requestCount,
                Url.Action("Index", "Search", new { area = string.Empty }) ?? "/immobili"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Remove(Guid id)
        {
            var userId = AuthenticatedUserId.Get(User);
            if (!userId.HasValue)
            {
                return Challenge();
            }

            await removeClientSavedSearchCommand.ExecuteAsync(
                userId.Value,
                id,
                HttpContext.RequestAborted);

            return RedirectToAction(nameof(Index));
        }
    }
}
