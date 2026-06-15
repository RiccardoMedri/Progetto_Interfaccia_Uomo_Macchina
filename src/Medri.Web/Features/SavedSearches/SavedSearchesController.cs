using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Threading.Tasks;
using Medri.Services.Medri.Application;
using Medri.Web.Infrastructure;

namespace Medri.Web.Features.SavedSearches
{
    public partial class SavedSearchesController : Controller
    {
        private readonly SaveClientSearchCommand saveClientSearchCommand;

        public SavedSearchesController(SaveClientSearchCommand saveClientSearchCommand)
        {
            this.saveClientSearchCommand = saveClientSearchCommand;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Save(SaveSearchInputModel input)
        {
            input ??= new SaveSearchInputModel();

            var returnUrl = string.IsNullOrWhiteSpace(input.ReturnUrl) || !Url.IsLocalUrl(input.ReturnUrl)
                ? DefaultSearchReturnUrl()
                : input.ReturnUrl;

            var userId = AuthenticatedUserId.Get(User);
            if (!userId.HasValue)
            {
                PendingSavedSearchSession.Store(HttpContext.Session, input, returnUrl);
                return RedirectToPendingSavedSearchLogin();
            }

            await saveClientSearchCommand.ExecuteAsync(
                userId.Value,
                PendingSavedSearchSession.ToRequest(input),
                HttpContext.RequestAborted);

            if (IsAjaxRequest())
            {
                return Json(new { succeeded = true });
            }

            return RedirectToSavedSearch(returnUrl);
        }

        [HttpGet]
        public virtual async Task<IActionResult> CompletePending()
        {
            var userId = AuthenticatedUserId.Get(User);
            if (!userId.HasValue)
            {
                return RedirectToPendingSavedSearchLogin();
            }

            if (!PendingSavedSearchSession.TryRead(HttpContext.Session, out var request, out var returnUrl))
            {
                return RedirectToAction("Index", "Search");
            }

            PendingSavedSearchSession.Clear(HttpContext.Session);
            await saveClientSearchCommand.ExecuteAsync(
                userId.Value,
                request,
                HttpContext.RequestAborted);

            return RedirectToAction(
                "Index",
                "Searches",
                new { area = "Client", saved = "1" });
        }

        private RedirectResult RedirectToSavedSearch(string returnUrl)
        {
            var targetUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : DefaultSearchReturnUrl();
            return Redirect(QueryHelpers.AddQueryString(targetUrl, "savedSearch", "1"));
        }

        private string DefaultSearchReturnUrl()
        {
            return Url.Action("Index", "Search") ?? "/immobili";
        }

        private RedirectToActionResult RedirectToPendingSavedSearchLogin()
        {
            return RedirectToAction(
                "Login",
                "Login",
                new { returnUrl = Url.Action(nameof(CompletePending), "SavedSearches") });
        }

        private bool IsAjaxRequest()
        {
            return string.Equals(
                Request.Headers["X-Requested-With"],
                "XMLHttpRequest",
                System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
