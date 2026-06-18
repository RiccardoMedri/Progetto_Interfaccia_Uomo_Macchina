using Medri.Web.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Medri.Infrastructure;
using Medri.Services.Medri.Application;
using Medri.Services.Medri.Identity;

namespace Medri.Web.Features.Login
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous]
    [Alerts]
    [ModelStateToTempData]
    public partial class LoginController : Controller
    {
        public static string LoginErrorModelStateKey = "LoginError";
        private readonly AccountService accountService;
        private readonly AddFavoritePropertyCommand addFavoritePropertyCommand;
        private readonly ClaimClientRequestsCommand claimClientRequestsCommand;

        public LoginController(
            AccountService accountService,
            AddFavoritePropertyCommand addFavoritePropertyCommand,
            ClaimClientRequestsCommand claimClientRequestsCommand)
        {
            this.accountService = accountService;
            this.addFavoritePropertyCommand = addFavoritePropertyCommand;
            this.claimClientRequestsCommand = claimClientRequestsCommand;
        }

        private async Task<ActionResult> LoginAndRedirect(
            UserDetailDTO utente,
            string returnUrl,
            bool rememberMe,
            Guid? pendingFavoritePropertyId)
        {
            var displayName = $"{utente.FirstName} {utente.LastName}".Trim();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, utente.Id.ToString()),
                new Claim(ClaimTypes.Email, utente.Email),
                new Claim(
                    ClaimTypes.Name,
                    string.IsNullOrWhiteSpace(displayName) ? utente.Email : displayName)
            };

            if (!string.IsNullOrWhiteSpace(utente.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, utente.Role));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
            {
                ExpiresUtc = (rememberMe) ? DateTimeOffset.UtcNow.AddMonths(3) : null,
                IsPersistent = rememberMe,
            });

            if (pendingFavoritePropertyId.HasValue)
            {
                await addFavoritePropertyCommand.ExecuteAsync(
                    utente.Id,
                    pendingFavoritePropertyId.Value,
                    HttpContext.RequestAborted);
            }

            if (utente.Role == UserRoles.Client)
            {
                await ClaimPendingClientRequestsAsync(utente.Id);
            }

            var localReturnUrl = NormalizeLocalReturnUrl(returnUrl);
            if (localReturnUrl != null)
                return Redirect(localReturnUrl);

            if (utente.Role == UserRoles.Admin || utente.Role == UserRoles.Operator)
                return Redirect("/admin");

            return Redirect("/client-saved");
        }

        [HttpGet]
        public virtual IActionResult Login(string returnUrl, Guid? pendingFavoritePropertyId)
        {
            if (HttpContext.User != null && HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
            {
                var authenticatedReturnUrl = NormalizeLocalReturnUrl(returnUrl);
                if (authenticatedReturnUrl != null)
                    return Redirect(authenticatedReturnUrl);

                return RedirectToDefaultArea();
            }

            var localReturnUrl = NormalizeLocalReturnUrl(returnUrl);
            RemoveAuthReturnUrlModelState();

            return View("Auth", BuildAuthPage(AuthModes.Login, localReturnUrl, pendingFavoritePropertyId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async virtual Task<ActionResult> Login([Bind(Prefix = nameof(AuthPageViewModel.Login))] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var utente = await accountService.CheckCredentialsAsync(new CheckLoginCredentialsQuery
                    {
                        Email = model.Email,
                        Password = model.Password,
                    }, HttpContext.RequestAborted);

                    return await LoginAndRedirect(
                        utente,
                        model.ReturnUrl,
                        model.RememberMe,
                        model.PendingFavoritePropertyId);
                }
                catch (LoginException e)
                {
                    ModelState.AddModelError($"{nameof(AuthPageViewModel.Login)}.{LoginErrorModelStateKey}", e.Message);
                }
            }

            return RedirectToAction(nameof(Login), new
            {
                returnUrl = NormalizeLocalReturnUrl(model.ReturnUrl),
                pendingFavoritePropertyId = model.PendingFavoritePropertyId
            });
        }

        [HttpGet]
        public virtual IActionResult Register(string returnUrl, Guid? pendingFavoritePropertyId)
        {
            if (HttpContext.User != null && HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
            {
                var authenticatedReturnUrl = NormalizeLocalReturnUrl(returnUrl);
                if (authenticatedReturnUrl != null)
                    return Redirect(authenticatedReturnUrl);

                return RedirectToDefaultArea();
            }

            var localReturnUrl = NormalizeLocalReturnUrl(returnUrl);
            RemoveAuthReturnUrlModelState();

            return View("Auth", BuildAuthPage(AuthModes.Register, localReturnUrl, pendingFavoritePropertyId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async virtual Task<ActionResult> Register([Bind(Prefix = nameof(AuthPageViewModel.Register))] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var utente = await accountService.RegisterClientAsync(new RegisterClientUserCommand
                    {
                        Email = model.Email,
                        Password = model.Password,
                        FirstName = model.FirstName,
                        LastName = model.LastName
                    }, HttpContext.RequestAborted);

                    return await LoginAndRedirect(
                        utente,
                        model.ReturnUrl,
                        rememberMe: false,
                        model.PendingFavoritePropertyId);
                }
                catch (LoginException e)
                {
                    ModelState.AddModelError($"{nameof(AuthPageViewModel.Register)}.{LoginErrorModelStateKey}", e.Message);
                }
            }

            return RedirectToAction(nameof(Register), new
            {
                returnUrl = NormalizeLocalReturnUrl(model.ReturnUrl),
                pendingFavoritePropertyId = model.PendingFavoritePropertyId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async virtual Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            Alerts.AddSuccess(this, "Utente scollegato correttamente");
            return RedirectToAction(MVC.Login.Login());
        }

        private string NormalizeLocalReturnUrl(string returnUrl)
        {
            return Url.IsLocalUrl(returnUrl) ? returnUrl : null;
        }

        private IActionResult RedirectToDefaultArea()
        {
            if (User.IsInRole(UserRoles.Admin) || User.IsInRole(UserRoles.Operator))
            {
                return Redirect("/admin");
            }

            return Redirect("/client-saved");
        }

        private AuthPageViewModel BuildAuthPage(string mode, string returnUrl, Guid? pendingFavoritePropertyId)
        {
            return new AuthPageViewModel
            {
                Mode = mode,
                Login = new LoginViewModel
                {
                    ReturnUrl = returnUrl,
                    PendingFavoritePropertyId = pendingFavoritePropertyId
                },
                Register = new RegisterViewModel
                {
                    ReturnUrl = returnUrl,
                    PendingFavoritePropertyId = pendingFavoritePropertyId
                }
            };
        }

        private async Task ClaimPendingClientRequestsAsync(Guid userId)
        {
            var pendingRequestIds = PendingClientRequestSession.Read(HttpContext.Session);
            if (pendingRequestIds.Count == 0)
            {
                return;
            }

            await claimClientRequestsCommand.ExecuteAsync(
                userId,
                pendingRequestIds,
                HttpContext.RequestAborted);
            PendingClientRequestSession.Clear(HttpContext.Session);
        }

        private void RemoveAuthReturnUrlModelState()
        {
            ModelState.Remove(nameof(LoginViewModel.ReturnUrl));
            ModelState.Remove(nameof(RegisterViewModel.ReturnUrl));
            ModelState.Remove($"{nameof(AuthPageViewModel.Login)}.{nameof(LoginViewModel.ReturnUrl)}");
            ModelState.Remove($"{nameof(AuthPageViewModel.Register)}.{nameof(RegisterViewModel.ReturnUrl)}");
            ModelState.Remove(nameof(LoginViewModel.PendingFavoritePropertyId));
            ModelState.Remove(nameof(RegisterViewModel.PendingFavoritePropertyId));
            ModelState.Remove($"{nameof(AuthPageViewModel.Login)}.{nameof(LoginViewModel.PendingFavoritePropertyId)}");
            ModelState.Remove($"{nameof(AuthPageViewModel.Register)}.{nameof(RegisterViewModel.PendingFavoritePropertyId)}");
        }
    }
}
