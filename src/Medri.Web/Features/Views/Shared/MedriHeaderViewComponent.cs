using Microsoft.AspNetCore.Mvc;
using System;
using Medri.Services.Medri.Identity;

namespace Medri.Web.Features.Views.Shared
{
    public sealed class MedriHeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var model = MedriHeaderViewModel.Create(
                ViewData["NavSection"] as string,
                RouteData.Values["area"]?.ToString(),
                RouteData.Values["controller"]?.ToString(),
                RouteData.Values["action"]?.ToString(),
                HttpContext.User.Identity?.IsAuthenticated == true,
                HttpContext.User.IsInRole(UserRoles.Admin) ||
                    HttpContext.User.IsInRole(UserRoles.Operator));

            return View("~/Features/Views/Shared/_MedriResponsiveHeader.cshtml", model);
        }
    }

    public sealed class MedriHeaderViewModel
    {
        public bool IsAuthenticated { get; private set; }

        public bool IsAdminAreaUser { get; private set; }

        public bool IsBuySection { get; private set; }

        public bool IsRentSection { get; private set; }

        public bool IsOwnerSection { get; private set; }

        public bool IsSellPage { get; private set; }

        public bool IsRentOutPage { get; private set; }

        public bool IsValuationPage { get; private set; }

        public bool IsMyAreaSection { get; private set; }

        public string AccountLabel => IsAuthenticated
            ? IsAdminAreaUser ? "Area riservata" : "Area cliente"
            : "Accedi";

        public static MedriHeaderViewModel Create(
            string navSection,
            string area,
            string controller,
            string action,
            bool isAuthenticated,
            bool isAdminAreaUser)
        {
            var isLeadIntake = EqualsOrdinal(controller, "LeadIntake");
            var isBuyPage = isLeadIntake && EqualsOrdinal(action, "Buy");
            var isRentPage = isLeadIntake && EqualsOrdinal(action, "Rent");
            var isSellPage = isLeadIntake && EqualsOrdinal(action, "Sell");
            var isRentOutPage = isLeadIntake && EqualsOrdinal(action, "RentOut");
            var isValuationPage = isLeadIntake && EqualsOrdinal(action, "Valuation");
            var isClientSection = EqualsOrdinal(area, "Client");
            var isAdminSection = EqualsOrdinal(area, "Admin");

            return new MedriHeaderViewModel
            {
                IsAuthenticated = isAuthenticated,
                IsAdminAreaUser = isAdminAreaUser,
                IsBuySection = EqualsOrdinal(navSection, "Buy") || isBuyPage,
                IsRentSection = EqualsOrdinal(navSection, "Rent") || isRentPage,
                IsOwnerSection = EqualsOrdinal(navSection, "Owner") ||
                    isSellPage ||
                    isRentOutPage ||
                    isValuationPage,
                IsSellPage = isSellPage,
                IsRentOutPage = isRentOutPage,
                IsValuationPage = isValuationPage,
                IsMyAreaSection = isAuthenticated &&
                    (isAdminAreaUser ? isAdminSection : isClientSection)
            };
        }

        private static bool EqualsOrdinal(string left, string right)
        {
            return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
        }
    }
}
