using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Medri.Services.Medri.Application;
using Medri.Web.Infrastructure;

namespace Medri.Web.Areas.Client.Notifications
{
    [Area("Client")]
    public partial class NotificationsController : ClientAreaController
    {
        private readonly ClientNotificationPreferencesQuery preferencesQuery;
        private readonly UpdateClientNotificationPreferenceCommand updatePreferenceCommand;

        public NotificationsController(
            ClientNotificationPreferencesQuery preferencesQuery,
            UpdateClientNotificationPreferenceCommand updatePreferenceCommand)
        {
            this.preferencesQuery = preferencesQuery;
            this.updatePreferenceCommand = updatePreferenceCommand;
        }

        [HttpGet]
        public virtual async Task<IActionResult> Index()
        {
            var userId = AuthenticatedUserId.Get(User);
            if (!userId.HasValue)
            {
                return Challenge();
            }

            var result = await preferencesQuery.ExecuteAsync(
                userId.Value,
                HttpContext.RequestAborted);
            return View(ClientNotificationsViewModelMapper.Create(result));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Update(
            ClientNotificationPreferenceInputModel input)
        {
            var userId = AuthenticatedUserId.Get(User);
            if (!userId.HasValue)
            {
                return Challenge();
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            await updatePreferenceCommand.ExecuteAsync(
                userId.Value,
                ClientNotificationsViewModelMapper.CreateCommand(input),
                HttpContext.RequestAborted);

            return RedirectToAction(nameof(Index));
        }
    }
}
