
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using portal.mps.Data;
using portal.mps.Models;
using portal.mps.Models.ViewModels;

namespace portal.mps.Controllers
{
    public class AuthController : Controller
    {
        private UserManager<mpsUser> _manager;
        private SignInManager<mpsUser> _signInManager;

        private ILogger<AuthController> _logger;

        public AuthController(UserManager<mpsUser> manager, SignInManager<mpsUser> signinmanager,ILogger<AuthController> logger)
        {
            _manager = manager;
            _signInManager=signinmanager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            if(this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index","Home");
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"{model.UserName} logged in at {DateTime.Now.ToString("dd-MM-yy hh:mm")}");
                    return RedirectToLocal(returnUrl);
                    // if(Request.Query.Keys.Contains("ReturnUrl"))
                    // {
                    //     var s = Request.QueryString.Value.ToString();
                    // }
                    // return RedirectToAction(nameof(HomeController.Index), "Home");
                }
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("","Username or password is incorrect");
            return View(model);
        }

         [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        public IActionResult AccessDenied()
        {
            _logger.LogInformation("Access denied.");
            return View();
        }
        public async Task<JsonResult> checkUserName(string n){
            bool res=true;
            var existingAccount = await _manager.FindByNameAsync(n);
            if(existingAccount!=null){res=false;}
            return Json(res);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}