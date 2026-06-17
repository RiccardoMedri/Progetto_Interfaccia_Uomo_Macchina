using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Medri.Services.Medri.Application;
using Medri.Web.Infrastructure;

namespace Medri.Web.Areas.Admin.Properties
{
    [Area("Admin")]
    public partial class PropertiesController : AdminAreaController
    {
        private readonly AdminPropertyListQuery adminPropertyListQuery;
        private readonly AdminPropertyDetailQuery adminPropertyDetailQuery;
        private readonly AdminCreatePropertyCommand adminCreatePropertyCommand;
        private readonly UpdateAdminPropertyDetailCommand updateAdminPropertyDetailCommand;
        private readonly MarkAdminPropertyReadyCommand markAdminPropertyReadyCommand;
        private readonly PublishAdminPropertyCommand publishAdminPropertyCommand;
        private readonly ArchiveAdminPropertyCommand archiveAdminPropertyCommand;
        private readonly DiscardDraftAdminPropertyCommand discardDraftAdminPropertyCommand;
        private readonly BulkAssignAdminPropertiesCommand bulkAssignAdminPropertiesCommand;
        private readonly FeatureAdminPropertyCommand featureAdminPropertyCommand;
        private readonly MoveFeaturedAdminPropertyCommand moveFeaturedAdminPropertyCommand;
        private readonly RemoveFeaturedAdminPropertyCommand removeFeaturedAdminPropertyCommand;
        private readonly AddAdminPropertyMediaCommand addAdminPropertyMediaCommand;
        private readonly IAdminPropertyMediaStorage propertyMediaStorage;
        private readonly IAdminPropertyGeocoder propertyGeocoder;

        public PropertiesController(
            AdminPropertyListQuery adminPropertyListQuery,
            AdminPropertyDetailQuery adminPropertyDetailQuery,
            AdminCreatePropertyCommand adminCreatePropertyCommand,
            UpdateAdminPropertyDetailCommand updateAdminPropertyDetailCommand,
            MarkAdminPropertyReadyCommand markAdminPropertyReadyCommand,
            PublishAdminPropertyCommand publishAdminPropertyCommand,
            ArchiveAdminPropertyCommand archiveAdminPropertyCommand,
            DiscardDraftAdminPropertyCommand discardDraftAdminPropertyCommand,
            BulkAssignAdminPropertiesCommand bulkAssignAdminPropertiesCommand,
            FeatureAdminPropertyCommand featureAdminPropertyCommand,
            MoveFeaturedAdminPropertyCommand moveFeaturedAdminPropertyCommand,
            RemoveFeaturedAdminPropertyCommand removeFeaturedAdminPropertyCommand,
            AddAdminPropertyMediaCommand addAdminPropertyMediaCommand,
            IAdminPropertyMediaStorage propertyMediaStorage,
            IAdminPropertyGeocoder propertyGeocoder)
        {
            this.adminPropertyListQuery = adminPropertyListQuery;
            this.adminPropertyDetailQuery = adminPropertyDetailQuery;
            this.adminCreatePropertyCommand = adminCreatePropertyCommand;
            this.updateAdminPropertyDetailCommand = updateAdminPropertyDetailCommand;
            this.markAdminPropertyReadyCommand = markAdminPropertyReadyCommand;
            this.publishAdminPropertyCommand = publishAdminPropertyCommand;
            this.archiveAdminPropertyCommand = archiveAdminPropertyCommand;
            this.discardDraftAdminPropertyCommand = discardDraftAdminPropertyCommand;
            this.bulkAssignAdminPropertiesCommand = bulkAssignAdminPropertiesCommand;
            this.featureAdminPropertyCommand = featureAdminPropertyCommand;
            this.moveFeaturedAdminPropertyCommand = moveFeaturedAdminPropertyCommand;
            this.removeFeaturedAdminPropertyCommand = removeFeaturedAdminPropertyCommand;
            this.addAdminPropertyMediaCommand = addAdminPropertyMediaCommand;
            this.propertyMediaStorage = propertyMediaStorage;
            this.propertyGeocoder = propertyGeocoder;
        }

        [HttpGet]
        public virtual async Task<IActionResult> Index(AdminPropertyListInputModel input)
        {
            input ??= new AdminPropertyListInputModel();

            var result = await adminPropertyListQuery.ExecuteAsync(
                input.ToQueryFilter(),
                HttpContext.RequestAborted);

            return View(AdminPropertyListViewModelMapper.Create(result, input, User));
        }

        // Tracks the draft auto-created in the CURRENT creation flow (media upload before save),
        // so "Esci senza salvare" can discard only that one and never a pre-existing incomplete listing.
        private const string PendingDraftSessionKey = "admin:pendingDraftProperty";

        [HttpGet]
        public virtual async Task<IActionResult> Create()
        {
            HttpContext.Session.Remove(PendingDraftSessionKey);

            var result = await adminPropertyDetailQuery.CreateNewAsync(
                HttpContext.RequestAborted);

            return View("Edit", AdminPropertyDetailViewModelMapper.CreateEdit(result, null, User));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Geocode(AdminPropertyGeocodeInputModel input)
        {
            input ??= new AdminPropertyGeocodeInputModel();

            var result = await propertyGeocoder.GeocodeAsync(
                input.Address,
                input.DisplayLocation,
                HttpContext.RequestAborted);

            return Json(new
            {
                succeeded = result.Succeeded,
                latitude = result.Latitude,
                longitude = result.Longitude,
                message = result.Message
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(AdminPropertyDetailInputModel input)
        {
            var isMediaUploadAction = IsMediaUploadAction();

            // Media upload during creation: persist a draft (skipping the full required-field
            // validation) so the operator can upload media, see the cards and reorder them,
            // and only afterwards fill the remaining fields before the final save. Mirrors Update().
            if (!ModelState.IsValid && isMediaUploadAction && HasUploadedMedia(input))
            {
                var draftResult = await adminCreatePropertyCommand.ExecuteAsync(
                    input.ToUpdateDto(),
                    HttpContext.RequestAborted);

                await SaveUploadedMediaAsync(draftResult.Reference, input);

                HttpContext.Session.SetString(PendingDraftSessionKey, draftResult.Reference);
                return RedirectToAction(nameof(Edit), new { reference = draftResult.Reference });
            }

            if (!ModelState.IsValid)
            {
                var result = await adminPropertyDetailQuery.CreateNewAsync(
                    HttpContext.RequestAborted);

                return View("Edit", AdminPropertyDetailViewModelMapper.CreateEdit(result, input, User));
            }

            var commandResult = await adminCreatePropertyCommand.ExecuteAsync(
                input.ToUpdateDto(),
                HttpContext.RequestAborted);

            await SaveUploadedMediaAsync(commandResult.Reference, input);

            if (!isMediaUploadAction)
            {
                Alerts.AddSuccess(this, $"Immobile {commandResult.Reference} creato.");
            }

            return RedirectToAction(nameof(Edit), new { reference = commandResult.Reference });
        }

        [HttpGet]
        public virtual async Task<IActionResult> Edit(string reference)
        {
            var result = await adminPropertyDetailQuery.ExecuteAsync(
                reference,
                HttpContext.RequestAborted);

            if (result == null)
            {
                return NotFound();
            }

            // "Esci senza salvare" discards a draft ONLY if it is the one auto-created in the current
            // creation flow (session-tracked); a pre-existing incomplete listing is never deleted.
            var pendingDraftReference = HttpContext.Session.GetString(PendingDraftSessionKey);
            var canDiscardDraft = pendingDraftReference != null &&
                string.Equals(pendingDraftReference, result.Reference, StringComparison.Ordinal);
            if (!canDiscardDraft && pendingDraftReference != null)
            {
                HttpContext.Session.Remove(PendingDraftSessionKey);
            }

            var viewModel = AdminPropertyDetailViewModelMapper.CreateEdit(result, null, User);
            viewModel.CanDiscardDraft = canDiscardDraft;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Update(
            string reference,
            AdminPropertyDetailInputModel input)
        {
            var isMediaUploadAction = IsMediaUploadAction();
            var result = await adminPropertyDetailQuery.ExecuteAsync(
                reference,
                HttpContext.RequestAborted);

            if (result == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid && isMediaUploadAction && HasUploadedMedia(input))
            {
                await SaveUploadedMediaAsync(result.Reference, input);
                return RedirectToAction(nameof(Edit), new { reference = result.Reference });
            }

            if (!ModelState.IsValid)
            {
                return View("Edit", AdminPropertyDetailViewModelMapper.CreateEdit(result, input, User));
            }

            var commandResult = await updateAdminPropertyDetailCommand.ExecuteAsync(
                reference,
                input.ToUpdateDto(),
                HttpContext.RequestAborted);

            if (!commandResult.Succeeded)
            {
                return NotFound();
            }

            await SaveUploadedMediaAsync(commandResult.Reference, input);

            if (!isMediaUploadAction)
            {
                // Explicit save = the creation is finalized; it can no longer be auto-discarded.
                HttpContext.Session.Remove(PendingDraftSessionKey);
                Alerts.AddSuccess(this, $"Immobile {commandResult.Reference} aggiornato.");
            }

            return RedirectToAction(nameof(Edit), new { reference = commandResult.Reference });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> MarkReady(string reference)
        {
            var commandResult = await markAdminPropertyReadyCommand.ExecuteAsync(
                reference,
                HttpContext.RequestAborted);

            if (commandResult.IsBlocked)
            {
                Alerts.AddWarning(this, $"Immobile {commandResult.Reference} non pronto: {commandResult.Message}.");
                return RedirectToAction(nameof(Edit), new { reference = commandResult.Reference });
            }

            if (!commandResult.Succeeded)
            {
                return NotFound();
            }

            Alerts.AddSuccess(this, $"Immobile {commandResult.Reference} segnato come pronto.");
            return RedirectToAction(nameof(Preview), new { reference = commandResult.Reference });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Publish(
            string reference,
            AdminPropertyListInputModel input)
        {
            var commandResult = await publishAdminPropertyCommand.ExecuteAsync(
                reference,
                HttpContext.RequestAborted);

            if (!commandResult.Succeeded && !commandResult.IsBlocked)
            {
                return NotFound();
            }

            if (commandResult.IsBlocked)
            {
                Alerts.AddWarning(this, $"Immobile {commandResult.Reference} non pubblicato: {commandResult.Message}.");
            }
            else
            {
                Alerts.AddSuccess(this, $"Immobile {commandResult.Reference} pubblicato.");
            }

            return RedirectAfterListAction(commandResult.Reference, input);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Archive(
            string reference,
            AdminPropertyListInputModel input)
        {
            var commandResult = await archiveAdminPropertyCommand.ExecuteAsync(
                reference,
                HttpContext.RequestAborted);

            if (!commandResult.Succeeded)
            {
                return NotFound();
            }

            Alerts.AddSuccess(this, $"Immobile {commandResult.Reference} archiviato.");
            return RedirectAfterListAction(commandResult.Reference, input);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Discard(string reference)
        {
            // "Esci senza salvare": elimina SOLO la bozza auto-creata in questa sessione di creazione
            // (tracciata in sessione) e solo se ancora incompleta; altrimenti esce e basta.
            var pendingDraftReference = HttpContext.Session.GetString(PendingDraftSessionKey);
            if (pendingDraftReference != null &&
                string.Equals(pendingDraftReference, reference, StringComparison.Ordinal))
            {
                await discardDraftAdminPropertyCommand.ExecuteAsync(
                    reference,
                    HttpContext.RequestAborted);
            }

            HttpContext.Session.Remove(PendingDraftSessionKey);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> BulkAssign(AdminPropertyBulkActionInputModel input)
        {
            input ??= new AdminPropertyBulkActionInputModel();

            var result = await bulkAssignAdminPropertiesCommand.ExecuteAsync(
                input.SelectedPropertyReferences,
                input.BulkAssignedAgencyUserId,
                HttpContext.RequestAborted);

            if (result.HasUpdates)
            {
                Alerts.AddSuccess(this, $"{result.UpdatedCount} immobili assegnati.");
            }
            else
            {
                Alerts.AddWarning(this, "Nessun immobile aggiornato: controlla selezione e referente.");
            }

            return RedirectToAction(nameof(Index), input.ToRouteValues());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Feature(
            string reference,
            AdminPropertyListInputModel input)
        {
            var commandResult = await featureAdminPropertyCommand.ExecuteAsync(
                reference,
                HttpContext.RequestAborted);

            if (!commandResult.Succeeded && !commandResult.IsBlocked)
            {
                return NotFound();
            }

            if (commandResult.IsBlocked)
            {
                Alerts.AddWarning(this, $"Immobile {commandResult.Reference} non inserito in homepage: {commandResult.Message}.");
            }
            return RedirectToAction(nameof(Index), input?.ToRouteValues());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> MoveFeature(
            string reference,
            int direction)
        {
            var commandResult = await moveFeaturedAdminPropertyCommand.ExecuteAsync(
                reference,
                direction,
                HttpContext.RequestAborted);

            if (!commandResult.Succeeded)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> RemoveFeature(string reference)
        {
            var commandResult = await removeFeaturedAdminPropertyCommand.ExecuteAsync(
                reference,
                HttpContext.RequestAborted);

            if (!commandResult.Succeeded)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public virtual async Task<IActionResult> Preview(
            string reference,
            string mode = "desktop")
        {
            var result = await adminPropertyDetailQuery.ExecuteAsync(
                reference,
                HttpContext.RequestAborted);

            if (result == null)
            {
                return NotFound();
            }

            return View(AdminPropertyDetailViewModelMapper.CreatePreview(result, mode, User));
        }

        [HttpGet]
        public virtual async Task<IActionResult> PreviewMap(string reference)
        {
            var result = await adminPropertyDetailQuery.ExecuteAsync(
                reference,
                HttpContext.RequestAborted);

            if (result == null)
            {
                return NotFound();
            }

            return View(AdminPropertyDetailViewModelMapper.CreatePreviewMap(result, User));
        }

        private IActionResult RedirectAfterListAction(
            string reference,
            AdminPropertyListInputModel input)
        {
            if (HasListState(input))
            {
                return RedirectToAction(nameof(Index), input.ToRouteValues());
            }

            return RedirectToAction(nameof(Edit), new { reference });
        }

        private async Task SaveUploadedMediaAsync(
            string reference,
            AdminPropertyDetailInputModel input)
        {
            var result = await propertyMediaStorage.SaveAsync(
                reference,
                input?.UploadedMedia,
                HttpContext.RequestAborted);
            foreach (var url in result.SavedUrls)
            {
                await addAdminPropertyMediaCommand.ExecuteAsync(
                    reference,
                    url,
                    null,
                    HttpContext.RequestAborted);
            }

            if (result.RejectedCount > 0)
            {
                Alerts.AddWarning(
                    this,
                    result.RejectedCount == 1
                        ? "Un file non e stato caricato: usa JPG, PNG o WebP entro 8 MB."
                        : $"{result.RejectedCount.ToString(CultureInfo.InvariantCulture)} file non caricati: usa JPG, PNG o WebP entro 8 MB.");
            }
        }

        private bool IsMediaUploadAction()
        {
            return Request.HasFormContentType &&
                string.Equals(
                    Request.Form["SubmitAction"].ToString(),
                    "UploadMedia",
                    StringComparison.OrdinalIgnoreCase);
        }

        private static bool HasUploadedMedia(AdminPropertyDetailInputModel input)
        {
            return input?.UploadedMedia?.Any(file => file != null && file.Length > 0) == true;
        }

        private static bool HasListState(AdminPropertyListInputModel input)
        {
            return input != null &&
                (input.ReturnTo == "Index" ||
                 !string.IsNullOrWhiteSpace(input.SearchTerm) ||
                 !string.IsNullOrWhiteSpace(input.Status) ||
                 !string.IsNullOrWhiteSpace(input.Contract) ||
                 !string.IsNullOrWhiteSpace(input.Advisor) ||
                 input.Page > 1 ||
                 input.PageSize != 15);
        }

    }
}
