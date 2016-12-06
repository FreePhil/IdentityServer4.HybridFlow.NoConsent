using IdentityServer4.Services;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.Models;

namespace IdentityServer.Controllers
{
    public class LoginController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;

        public LoginController(IIdentityServerInteractionService interaction)
        {
            _interaction = interaction;
        }

        /// <summary>
        /// Show login page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = new LoginViewModel(HttpContext);

            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context != null)
            {
                // LoginHint contains username (OAuth2 authorize "login_hint" claim)
                vm.Username = context.LoginHint;
                vm.ReturnUrl = returnUrl;
            }

            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoLogin(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                //TODO: Add your own login logic
                if (model.Username == "test.user" && model.Password == "password")
                {
                    Guid userId = new Guid("8da49efb-a1aa-4253-bb7f-56cc6c532b78");

                    // Issue authentication cookie with subject ID and username
                    await HttpContext.Authentication.SignInAsync(userId.ToString(), model.Username);
                    
                    // Make sure the returnUrl is still valid, and if yes - redirect back to authorize endpoint
                    if (_interaction.IsValidReturnUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return Redirect("~/");
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }

            // Something went wrong, show form with error
            var vm = new LoginViewModel(HttpContext, model);
            return View("Index", vm);
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public IActionResult Logout(string logoutId)
        {
            var vm = new LogoutViewModel
            {
                LogoutId = logoutId
            };

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutViewModel model)
        {
            // Delete authentication cookie. This performs a single sign-out.
            await HttpContext.Authentication.SignOutAsync();

            // Set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // Get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(model.LogoutId);

            var vm = new LoggedOutViewModel
            {
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = logout?.ClientId,
                SignOutIframeUrl = logout?.SignOutIFrameUrl
            };

            return View("LoggedOut", vm);
        }
    }
}
