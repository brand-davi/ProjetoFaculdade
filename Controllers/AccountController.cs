using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManagerMvc.Models;
using ProjectManagerMvc.ViewModels;

namespace ProjectManagerMvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(vm);
            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Credenciais inválidas.");
                return View(vm);
            }

            var result = await _signInManager.PasswordSignInAsync(user, vm.Password, vm.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Credenciais inválidas.");
            return View(vm);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => Content("Acesso negado.");
    }
}
