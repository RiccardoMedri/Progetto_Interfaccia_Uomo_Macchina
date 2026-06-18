using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Medri.Services.Medri.Application;
using Medri.Web.Infrastructure;

namespace Medri.Web.Features.Visit
{
    public partial class VisitController : Controller
    {
        private readonly PropertyContactQuery propertyContactQuery;
        private readonly CreatePropertyContactRequestCommand createPropertyContactRequestCommand;
        private readonly SubmitPropertyContactRequestCommand submitPropertyContactRequestCommand;

        public VisitController(
            PropertyContactQuery propertyContactQuery,
            CreatePropertyContactRequestCommand createPropertyContactRequestCommand,
            SubmitPropertyContactRequestCommand submitPropertyContactRequestCommand)
        {
            this.propertyContactQuery = propertyContactQuery;
            this.createPropertyContactRequestCommand = createPropertyContactRequestCommand;
            this.submitPropertyContactRequestCommand = submitPropertyContactRequestCommand;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Start(string slug, PropertyContactStartViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                
                
                return RedirectToAction("Detail", "Property", new { slug });
            }

            var request = VisitViewModelMapper.Create(viewModel);
            request.ClientUserId = AuthenticatedUserId.Get(User);
            var appointmentId = await createPropertyContactRequestCommand.ExecuteAsync(
                slug,
                request,
                null,
                HttpContext.RequestAborted);
            return !appointmentId.HasValue
                ? NotFound()
                : RedirectToAction(nameof(Option), new { slug, appointmentId });
        }

        [HttpGet]
        public virtual async Task<IActionResult> Option(string slug, Guid? appointmentId)
        {
            if (appointmentId.HasValue)
            {
                var draft = await propertyContactQuery.FindReviewAsync(
                    slug,
                    appointmentId.Value,
                    HttpContext.RequestAborted);
                if (draft != null)
                {
                    return View(VisitViewModelMapper.CreateOption(draft));
                }

                return await propertyContactQuery.IsSubmittedAsync(
                    slug,
                    appointmentId.Value,
                    HttpContext.RequestAborted)
                    ? RedirectToConfirmation(slug, appointmentId.Value)
                    : NotFound();
            }

            var property = await propertyContactQuery.FindPropertyAsync(slug, HttpContext.RequestAborted);
            return property == null
                ? NotFound()
                : View(new VisitOptionViewModel { Property = VisitViewModelMapper.Create(property) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Option(string slug, VisitOptionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var property = await propertyContactQuery.FindPropertyAsync(slug, HttpContext.RequestAborted);
                viewModel.Property = property == null ? null : VisitViewModelMapper.Create(property);
                return viewModel.Property == null ? NotFound() : View(viewModel);
            }

            var request = VisitViewModelMapper.Create(viewModel);
            request.ClientUserId = AuthenticatedUserId.Get(User);
            var appointmentId = await createPropertyContactRequestCommand.ExecuteAsync(
                slug,
                request,
                viewModel.AppointmentId,
                HttpContext.RequestAborted);
            return !appointmentId.HasValue
                ? NotFound()
                : RedirectToAction(nameof(Review), new { slug, appointmentId });
        }

        [HttpGet]
        public virtual async Task<IActionResult> Review(string slug, Guid appointmentId)
        {
            var result = await propertyContactQuery.FindReviewAsync(slug, appointmentId, HttpContext.RequestAborted);
            if (result != null)
            {
                return View(VisitViewModelMapper.Create(result));
            }

            return await propertyContactQuery.IsSubmittedAsync(slug, appointmentId, HttpContext.RequestAborted)
                ? RedirectToConfirmation(slug, appointmentId)
                : NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Submit(string slug, Guid appointmentId)
        {
            var userId = AuthenticatedUserId.Get(User);
            var leadId = await submitPropertyContactRequestCommand.ExecuteAsync(
                slug,
                appointmentId,
                userId,
                HttpContext.RequestAborted);
            if (!leadId.HasValue)
            {
                return NotFound();
            }

            if (!userId.HasValue)
            {
                PendingClientRequestSession.Add(HttpContext.Session, leadId.Value);
            }

            return RedirectToConfirmation(slug, appointmentId);
        }

        [HttpGet]
        public virtual async Task<IActionResult> Confirmation(string slug, Guid appointmentId)
        {
            var result = await propertyContactQuery.FindConfirmationAsync(slug, appointmentId, HttpContext.RequestAborted);
            return result == null ? NotFound() : View(VisitViewModelMapper.Create(result));
        }

        private RedirectToActionResult RedirectToConfirmation(string slug, Guid appointmentId)
        {
            return RedirectToAction(nameof(Confirmation), new { slug, appointmentId });
        }
    }
}
