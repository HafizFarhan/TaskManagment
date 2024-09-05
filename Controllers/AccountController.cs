using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Injazat.DataAccess.Models;
using Injazat.Presentation.ModelViews.Account;
using Injazat.Presentation.ApiResponse;
using Injazat.Presentation.Services.UserService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Injazat.Presentation.Controllers;

namespace Injazat.Web.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: /Account/Login
        [HttpGet("Login")]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (IsJsonRequest())
            {
                // Return unauthenticated
                return Json(ApiResponse<string>.CreateFailure("Unauthenticated"));
            }
            return View();
        }

        // POST: /Account/Login
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "")
        {
            ViewData["ReturnUrl"] = returnUrl;
            bool isJsonRequest = IsJsonRequest();

            if (ModelState.IsValid)
            {
                var result = await _userService.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var response = ApiResponse<string>.CreateSuccess("Login successful", returnUrl);
                    if (isJsonRequest)
                    {
                        return Json(response);
                    }
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    var errorMessage = "Invalid login attempt.";
                    if (result.RequiresTwoFactor) errorMessage = "Requires two-factor authentication.";
                    if (result.IsLockedOut) errorMessage = "User account locked out.";
                    var errorResponse = ApiResponse<string>.CreateFailure(errorMessage);
                    if (isJsonRequest)
                    {
                        return Json(errorResponse);
                    }
                    ModelState.AddModelError(string.Empty, errorMessage);
                }
            }

            if (isJsonRequest)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage));
                return Json(ApiResponse<string>.CreateFailure(string.Join("; ", errors)));
            }

            return View(model);
        }

        // GET: /Account/AccessDenied
        [HttpGet("AccessDenied")]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            bool isJsonRequest = IsJsonRequest();

            _logger.LogWarning("Access denied attempt detected.");

            if (isJsonRequest)
            {
                var response = ApiResponse<string>.CreateFailure("Access denied. You do not have permission to access this resource.");
                return Json(response);
            }

            return View(); // Return the Access Denied view for web requests
        }

        // GET: /Account/Register
        [HttpGet("Register")]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            bool isJsonRequest = IsJsonRequest();

            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.Email, Email = model.Email };
                
                var result = await _userService.CreateUserAsync(user, model.Password, model.Role.ToString()); 

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    var signInResult = await _userService.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
                    var response = ApiResponse<string>.CreateSuccess("Registration successful");
                    if (isJsonRequest)
                    {
                        return Json(response);
                    }
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }
            if (isJsonRequest)
            {
                var errors = ApiResponse<string>.CreateFailure(string.Join("; ", ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage))));
                return Json(errors);
            }
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _userService.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/ForgotPassword
        [HttpGet("ForgotPassword")]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            bool isJsonRequest = IsJsonRequest();

            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByEmailAsync(model.Email);
                if (user == null || !(await _userService.IsEmailConfirmedAsync(user)))
                {
                    var response = ApiResponse<string>.CreateFailure("User does not exist or is not confirmed");
                    if (isJsonRequest) return Json(response);
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                var code = await _userService.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                if (isJsonRequest)
                {
                    return Json(ApiResponse<string>.CreateSuccess("Password reset email sent", callbackUrl));
                }
                return RedirectToAction("ForgotPasswordConfirmation");
            }
            if (isJsonRequest)
            {
                var errors = ApiResponse<string>.CreateFailure(string.Join("; ", ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage))));
                return Json(errors);
            }
            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet("ForgotPasswordConfirmation")]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [HttpGet("ResetPassword")]
        [AllowAnonymous]
        public IActionResult ResetPassword(string? code = null)
        {
            return code == null ? View("Error") : View();
        }

        // POST: /Account/ResetPassword
        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            bool isJsonRequest = IsJsonRequest();

            if (!ModelState.IsValid)
            {
                if (isJsonRequest)
                {
                    var errors = ApiResponse<string>.CreateFailure(string.Join("; ", ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage))));
                    return Json(errors);
                }
                return View(model);
            }
            var user = await _userService.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                if (isJsonRequest) return Json(ApiResponse<string>.CreateFailure("User not found"));
                return RedirectToAction("ResetPasswordConfirmation");
            }
            var result = await _userService.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                if (isJsonRequest) return Json(ApiResponse<string>.CreateSuccess("Password reset successful"));
                return RedirectToAction("ResetPasswordConfirmation");
            }
            AddErrors(result);
            if (isJsonRequest)
            {
                var errors = ApiResponse<string>.CreateFailure(string.Join("; ", ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage))));
                return Json(errors);
            }
            return View(model);
        }

        // GET: /Account/ResetPasswordConfirmation
        [HttpGet("ResetPasswordConfirmation")]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        private bool IsJsonRequest()
        {
            return Request.Headers["JsonPlease"].ToString().Contains("application/json");
        }

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
    }
}
