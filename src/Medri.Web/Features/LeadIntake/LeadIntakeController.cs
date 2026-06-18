using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Medri.Services.Medri.Application;
using Medri.Web.Infrastructure;

namespace Medri.Web.Features.LeadIntake
{
    public partial class LeadIntakeController : Controller
    {
        private readonly SubmitLeadRequestCommand submitLeadRequestCommand;
        private readonly LeadConfirmationQuery leadConfirmationQuery;

        public LeadIntakeController(
            SubmitLeadRequestCommand submitLeadRequestCommand,
            LeadConfirmationQuery leadConfirmationQuery)
        {
            this.submitLeadRequestCommand = submitLeadRequestCommand;
            this.leadConfirmationQuery = leadConfirmationQuery;
        }

        [HttpGet]
        public virtual IActionResult Buy()
        {
            return View(new BuyLeadRequestViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Buy(BuyLeadRequestViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var userId = AuthenticatedUserId.Get(User);
            return await SubmitAsync(LeadIntakeMapper.Create(viewModel), userId);
        }

        [HttpGet]
        public virtual IActionResult Rent()
        {
            return View(new RentLeadRequestViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Rent(RentLeadRequestViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var userId = AuthenticatedUserId.Get(User);
            return await SubmitAsync(LeadIntakeMapper.Create(viewModel), userId);
        }

        [HttpGet]
        public virtual IActionResult Sell()
        {
            return View(new SellLeadRequestViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Sell(SellLeadRequestViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var userId = AuthenticatedUserId.Get(User);
            return await SubmitAsync(LeadIntakeMapper.Create(viewModel), userId);
        }

        [HttpGet]
        public virtual IActionResult RentOut()
        {
            return View(new RentOutLeadRequestViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> RentOut(RentOutLeadRequestViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var userId = AuthenticatedUserId.Get(User);
            return await SubmitAsync(LeadIntakeMapper.Create(viewModel), userId);
        }

        [HttpGet]
        public virtual IActionResult Valuation()
        {
            return View(new ValuationRequestViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Valuation(ValuationRequestViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var userId = AuthenticatedUserId.Get(User);
            return await SubmitAsync(LeadIntakeMapper.Create(viewModel), userId);
        }

        [HttpGet]
        public virtual async Task<IActionResult> CompletePending()
        {
            var userId = AuthenticatedUserId.Get(User);
            if (!userId.HasValue)
            {
                return RedirectToPendingLeadLogin();
            }

            if (!PendingLeadRequestSession.TryCreateRequest(HttpContext.Session, out var request))
            {
                return RedirectToAction("Index", "Home");
            }

            PendingLeadRequestSession.Clear(HttpContext.Session);
            return await SubmitAsync(request, userId.Value);
        }

        [HttpGet]
        public virtual async Task<IActionResult> Confirmation(Guid id)
        {
            var result = await leadConfirmationQuery.ExecuteAsync(id, HttpContext.RequestAborted);
            return result == null ? NotFound() : View(LeadIntakeMapper.Create(result));
        }

        private async Task<IActionResult> SubmitAsync(LeadRequestDto request, Guid? userId)
        {
            request.ClientUserId = userId;
            var leadId = await submitLeadRequestCommand.ExecuteAsync(
                request,
                HttpContext.RequestAborted);

            if (!userId.HasValue)
            {
                PendingClientRequestSession.Add(HttpContext.Session, leadId);
            }

            return RedirectToAction(nameof(Confirmation), new { id = leadId });
        }

        private RedirectToActionResult RedirectToPendingLeadLogin()
        {
            return RedirectToAction(
                "Login",
                "Login",
                new { returnUrl = Url.Action(nameof(CompletePending), "LeadIntake") });
        }
    }
}
