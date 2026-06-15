using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Medri.Services.Medri.Application;
using Medri.Web.Infrastructure;

namespace Medri.Web.Areas.Admin.Leads
{
    [Area("Admin")]
    public partial class LeadsController : AdminAreaController
    {
        private readonly AdminLeadListQuery adminLeadListQuery;
        private readonly AdminLeadDetailQuery adminLeadDetailQuery;
        private readonly AdminCreateLeadCommand adminCreateLeadCommand;
        private readonly UpdateAdminLeadDetailCommand updateAdminLeadDetailCommand;
        private readonly ConvertAdminLeadToRequestCommand convertAdminLeadToRequestCommand;
        private readonly ArchiveAdminLeadCommand archiveAdminLeadCommand;
        private readonly RestoreAdminLeadCommand restoreAdminLeadCommand;
        private readonly BulkAssignAdminLeadsCommand bulkAssignAdminLeadsCommand;
        private readonly BulkConvertAdminLeadsCommand bulkConvertAdminLeadsCommand;
        private readonly BulkArchiveAdminLeadsCommand bulkArchiveAdminLeadsCommand;

        public LeadsController(
            AdminLeadListQuery adminLeadListQuery,
            AdminLeadDetailQuery adminLeadDetailQuery,
            AdminCreateLeadCommand adminCreateLeadCommand,
            UpdateAdminLeadDetailCommand updateAdminLeadDetailCommand,
            ConvertAdminLeadToRequestCommand convertAdminLeadToRequestCommand,
            ArchiveAdminLeadCommand archiveAdminLeadCommand,
            RestoreAdminLeadCommand restoreAdminLeadCommand,
            BulkAssignAdminLeadsCommand bulkAssignAdminLeadsCommand,
            BulkConvertAdminLeadsCommand bulkConvertAdminLeadsCommand,
            BulkArchiveAdminLeadsCommand bulkArchiveAdminLeadsCommand)
        {
            this.adminLeadListQuery = adminLeadListQuery;
            this.adminLeadDetailQuery = adminLeadDetailQuery;
            this.adminCreateLeadCommand = adminCreateLeadCommand;
            this.updateAdminLeadDetailCommand = updateAdminLeadDetailCommand;
            this.convertAdminLeadToRequestCommand = convertAdminLeadToRequestCommand;
            this.archiveAdminLeadCommand = archiveAdminLeadCommand;
            this.restoreAdminLeadCommand = restoreAdminLeadCommand;
            this.bulkAssignAdminLeadsCommand = bulkAssignAdminLeadsCommand;
            this.bulkConvertAdminLeadsCommand = bulkConvertAdminLeadsCommand;
            this.bulkArchiveAdminLeadsCommand = bulkArchiveAdminLeadsCommand;
        }

        [HttpGet]
        public virtual async Task<IActionResult> Index(AdminLeadListInputModel input)
        {
            input ??= new AdminLeadListInputModel();

            var result = await adminLeadListQuery.ExecuteAsync(
                input.ToQueryFilter(),
                HttpContext.RequestAborted);

            return View(AdminLeadListViewModelMapper.Create(result, input, User));
        }

        [HttpGet]
        public virtual async Task<IActionResult> Create()
        {
            var result = await adminLeadDetailQuery.CreateNewAsync(
                HttpContext.RequestAborted);

            return View("Detail", AdminLeadDetailViewModelMapper.Create(result, null, User));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(AdminLeadDetailInputModel input)
        {
            if (!ModelState.IsValid)
            {
                var result = await adminLeadDetailQuery.CreateNewAsync(
                    HttpContext.RequestAborted);

                return View("Detail", AdminLeadDetailViewModelMapper.Create(result, input, User));
            }

            var commandResult = await adminCreateLeadCommand.ExecuteAsync(
                input.ToUpdateDto(),
                HttpContext.RequestAborted);

            Alerts.AddSuccess(this, $"Lead {commandResult.Reference} creato.");
            return RedirectToAction(nameof(Detail), new { reference = commandResult.Reference });
        }

        [HttpGet]
        public virtual async Task<IActionResult> Detail(string reference)
        {
            var result = await adminLeadDetailQuery.ExecuteAsync(
                reference,
                HttpContext.RequestAborted);

            if (result == null)
            {
                return NotFound();
            }

            return View(AdminLeadDetailViewModelMapper.Create(result, null, User));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Update(
            string reference,
            AdminLeadDetailInputModel input)
        {
            if (!ModelState.IsValid)
            {
                var result = await adminLeadDetailQuery.ExecuteAsync(
                    reference,
                    HttpContext.RequestAborted);

                if (result == null)
                {
                    return NotFound();
                }

                return View("Detail", AdminLeadDetailViewModelMapper.Create(result, input, User));
            }

            var commandResult = await updateAdminLeadDetailCommand.ExecuteAsync(
                reference,
                input.ToUpdateDto(),
                HttpContext.RequestAborted);

            if (!commandResult.Succeeded)
            {
                return NotFound();
            }

            if (commandResult.HasChanges)
            {
                Alerts.AddSuccess(this, $"Lead {commandResult.Reference} aggiornato.");
            }
            else
            {
                Alerts.AddInfo(this, "Nessuna modifica da salvare.");
            }

            return RedirectToAction(nameof(Detail), new { reference = commandResult.Reference });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Convert(
            string reference,
            AdminLeadListReturnInputModel input)
        {
            var commandResult = await convertAdminLeadToRequestCommand.ExecuteAsync(
                reference,
                HttpContext.RequestAborted);

            if (!commandResult.Succeeded)
            {
                return NotFound();
            }

            Alerts.AddSuccess(this, $"Lead {commandResult.Reference} convertito in richiesta.");

            if (input != null && input.ShouldReturnToIndex)
            {
                return RedirectToAction(nameof(Index), input.ToRouteValues());
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Archive(
            string reference,
            AdminLeadListReturnInputModel input)
        {
            var commandResult = await archiveAdminLeadCommand.ExecuteAsync(
                reference,
                HttpContext.RequestAborted);

            if (!commandResult.Succeeded)
            {
                return NotFound();
            }

            if (commandResult.HasChanges)
            {
                Alerts.AddSuccess(this, $"Lead {commandResult.Reference} archiviato.");
            }
            else
            {
                Alerts.AddInfo(this, "Lead gia archiviato.");
            }

            if (input != null && input.ShouldReturnToIndex)
            {
                return RedirectToAction(nameof(Index), input.ToRouteValues());
            }

            return RedirectToAction(nameof(Index), new { Status = "Archived" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Restore(string reference)
        {
            var commandResult = await restoreAdminLeadCommand.ExecuteAsync(
                reference,
                HttpContext.RequestAborted);

            if (!commandResult.Succeeded)
            {
                return NotFound();
            }

            if (commandResult.HasChanges)
            {
                Alerts.AddSuccess(this, $"Lead {commandResult.Reference} ripristinato.");
            }
            else
            {
                Alerts.AddInfo(this, "Il lead non era archiviato.");
            }

            return RedirectToAction(nameof(Detail), new { reference = commandResult.Reference });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> BulkAssign(AdminLeadBulkActionInputModel input)
        {
            input ??= new AdminLeadBulkActionInputModel();

            var result = await bulkAssignAdminLeadsCommand.ExecuteAsync(
                input.SelectedLeadReferences,
                input.BulkAssignedAgencyUserId,
                HttpContext.RequestAborted);

            AddBulkAlert(result, "assegnati");
            return RedirectToAction(nameof(Index), input.ToRouteValues());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> BulkConvert(AdminLeadBulkActionInputModel input)
        {
            input ??= new AdminLeadBulkActionInputModel();

            var result = await bulkConvertAdminLeadsCommand.ExecuteAsync(
                input.SelectedLeadReferences,
                HttpContext.RequestAborted);

            AddBulkAlert(result, "convertiti");
            return RedirectToAction(nameof(Index), input.ToRouteValues());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> BulkArchive(AdminLeadBulkActionInputModel input)
        {
            input ??= new AdminLeadBulkActionInputModel();

            var result = await bulkArchiveAdminLeadsCommand.ExecuteAsync(
                input.SelectedLeadReferences,
                HttpContext.RequestAborted);

            AddBulkAlert(result, "archiviati");
            return RedirectToAction(nameof(Index), input.ToRouteValues());
        }

        private void AddBulkAlert(
            AdminLeadBulkCommandResult result,
            string actionLabel)
        {
            if (result.HasUpdates)
            {
                Alerts.AddSuccess(this, $"{result.UpdatedCount} lead {actionLabel}.");
            }
            else
            {
                Alerts.AddWarning(this, "Nessun lead aggiornato: controlla selezione e campi richiesti.");
            }
        }
    }
}
