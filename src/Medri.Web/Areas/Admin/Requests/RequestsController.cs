using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Medri.Services.Medri.Application;
using Medri.Web.Infrastructure;

namespace Medri.Web.Areas.Admin.Requests
{
    [Area("Admin")]
    public partial class RequestsController : AdminAreaController
    {
        private readonly AdminRequestListQuery adminRequestListQuery;
        private readonly AdminRequestDetailQuery adminRequestDetailQuery;
        private readonly AdminCreateRequestCommand adminCreateRequestCommand;
        private readonly UpdateAdminRequestDetailCommand updateAdminRequestDetailCommand;
        private readonly ArchiveAdminRequestCommand archiveAdminRequestCommand;
        private readonly BulkAssignAdminRequestsCommand bulkAssignAdminRequestsCommand;

        public RequestsController(
            AdminRequestListQuery adminRequestListQuery,
            AdminRequestDetailQuery adminRequestDetailQuery,
            AdminCreateRequestCommand adminCreateRequestCommand,
            UpdateAdminRequestDetailCommand updateAdminRequestDetailCommand,
            ArchiveAdminRequestCommand archiveAdminRequestCommand,
            BulkAssignAdminRequestsCommand bulkAssignAdminRequestsCommand)
        {
            this.adminRequestListQuery = adminRequestListQuery;
            this.adminRequestDetailQuery = adminRequestDetailQuery;
            this.adminCreateRequestCommand = adminCreateRequestCommand;
            this.updateAdminRequestDetailCommand = updateAdminRequestDetailCommand;
            this.archiveAdminRequestCommand = archiveAdminRequestCommand;
            this.bulkAssignAdminRequestsCommand = bulkAssignAdminRequestsCommand;
        }

        [HttpGet]
        public virtual async Task<IActionResult> Index(AdminRequestListInputModel input)
        {
            input ??= new AdminRequestListInputModel();

            var result = await adminRequestListQuery.ExecuteAsync(
                input.ToQueryFilter(),
                HttpContext.RequestAborted);

            return View(AdminRequestListViewModelMapper.Create(result, input, User));
        }

        [HttpGet]
        public virtual async Task<IActionResult> Create()
        {
            var result = await adminRequestDetailQuery.CreateNewAsync(
                HttpContext.RequestAborted);

            return View("Edit", AdminRequestDetailViewModelMapper.Create(result, null, User));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(AdminRequestDetailInputModel input)
        {
            if (!ModelState.IsValid)
            {
                var result = await adminRequestDetailQuery.CreateNewAsync(
                    HttpContext.RequestAborted);

                return View("Edit", AdminRequestDetailViewModelMapper.Create(result, input, User));
            }

            var commandResult = await adminCreateRequestCommand.ExecuteAsync(
                input.ToUpdateDto(),
                HttpContext.RequestAborted);

            Alerts.AddSuccess(this, $"Richiesta {commandResult.Reference} creata.");
            return RedirectToAction(nameof(Edit), new { reference = commandResult.Reference });
        }

        [HttpGet]
        public virtual async Task<IActionResult> Edit(string reference)
        {
            var result = await adminRequestDetailQuery.ExecuteAsync(
                reference,
                HttpContext.RequestAborted);

            if (result == null)
            {
                return NotFound();
            }

            return View(AdminRequestDetailViewModelMapper.Create(result, null, User));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Update(
            string reference,
            AdminRequestDetailInputModel input)
        {
            if (!ModelState.IsValid)
            {
                var result = await adminRequestDetailQuery.ExecuteAsync(
                    reference,
                    HttpContext.RequestAborted);

                if (result == null)
                {
                    return NotFound();
                }

                return View("Edit", AdminRequestDetailViewModelMapper.Create(result, input, User));
            }

            var commandResult = await updateAdminRequestDetailCommand.ExecuteAsync(
                reference,
                input.ToUpdateDto(),
                HttpContext.RequestAborted);

            if (!commandResult.Succeeded)
            {
                return NotFound();
            }

            Alerts.AddSuccess(this, $"Richiesta {commandResult.Reference} aggiornata.");
            return RedirectToAction(nameof(Edit), new { reference = commandResult.Reference });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Archive(
            string reference,
            AdminRequestListInputModel input)
        {
            var commandResult = await archiveAdminRequestCommand.ExecuteAsync(
                reference,
                HttpContext.RequestAborted);

            if (!commandResult.Succeeded)
            {
                return NotFound();
            }

            Alerts.AddSuccess(this, $"Richiesta {commandResult.Reference} archiviata.");
            return RedirectToAction(nameof(Index), input?.ToRouteValues());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> BulkAssign(AdminRequestBulkActionInputModel input)
        {
            input ??= new AdminRequestBulkActionInputModel();

            var result = await bulkAssignAdminRequestsCommand.ExecuteAsync(
                input.SelectedRequestReferences,
                input.BulkAssignedAgencyUserId,
                HttpContext.RequestAborted);

            AddBulkAlert(result, "assegnate");
            return RedirectToAction(nameof(Index), input.ToRouteValues());
        }

        private void AddBulkAlert(
            AdminRequestBulkCommandResult result,
            string actionLabel)
        {
            if (result.HasUpdates)
            {
                Alerts.AddSuccess(this, $"{result.UpdatedCount} richieste {actionLabel}.");
            }
            else
            {
                Alerts.AddWarning(this, "Nessuna richiesta aggiornata: controlla selezione e campi richiesti.");
            }
        }
    }
}
