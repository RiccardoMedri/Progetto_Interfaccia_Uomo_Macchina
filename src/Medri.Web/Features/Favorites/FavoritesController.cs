using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Medri.Services.Medri.Application;
using Medri.Web.Infrastructure;

namespace Medri.Web.Features.Favorites
{
    public partial class FavoritesController : AuthenticatedBaseController
    {
        private readonly AddFavoritePropertyCommand addFavoritePropertyCommand;
        private readonly RemoveFavoritePropertyCommand removeFavoritePropertyCommand;

        public FavoritesController(
            AddFavoritePropertyCommand addFavoritePropertyCommand,
            RemoveFavoritePropertyCommand removeFavoritePropertyCommand)
        {
            this.addFavoritePropertyCommand = addFavoritePropertyCommand;
            this.removeFavoritePropertyCommand = removeFavoritePropertyCommand;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Add(Guid propertyId, string returnUrl)
        {
            var userId = AuthenticatedUserId.Get(User);
            if (!userId.HasValue)
            {
                return RedirectToLoginWithPendingFavorite(propertyId, returnUrl);
            }

            var result = await addFavoritePropertyCommand.ExecuteAsync(
                userId.Value,
                propertyId,
                HttpContext.RequestAborted);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return IsAjaxRequest()
                ? Json(result)
                : RedirectToLocal(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Remove(Guid propertyId, string returnUrl)
        {
            var userId = AuthenticatedUserId.Get(User);
            if (!userId.HasValue)
            {
                return Challenge();
            }

            var result = await removeFavoritePropertyCommand.ExecuteAsync(
                userId.Value,
                propertyId,
                HttpContext.RequestAborted);
            return IsAjaxRequest()
                ? Json(result)
                : RedirectToLocal(returnUrl);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            return Url.IsLocalUrl(returnUrl)
                ? Redirect(returnUrl)
                : RedirectToAction("Index", "Saved", new { area = "Client" });
        }

        private IActionResult RedirectToLoginWithPendingFavorite(Guid propertyId, string returnUrl)
        {
            var defaultReturnUrl = Url.Action("Index", "Search") ?? "/immobili";
            var localReturnUrl = Url.IsLocalUrl(returnUrl)
                ? returnUrl
                : defaultReturnUrl;

            return RedirectToAction(
                "Login",
                "Login",
                new
                {
                    returnUrl = localReturnUrl,
                    pendingFavoritePropertyId = propertyId
                });
        }

        private bool IsAjaxRequest()
        {
            return string.Equals(
                Request.Headers["X-Requested-With"],
                "XMLHttpRequest",
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
