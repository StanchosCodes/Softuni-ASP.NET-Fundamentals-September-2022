using Contacts.Data.Models;
using Contacts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public UserController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("All", "Contacts");
            }

            RegisterViewModel registerModel = new RegisterViewModel();

            return View(registerModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerModel);
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                UserName = registerModel.UserName,
                Email = registerModel.Email
            };

            var registerResult = await this.userManager.CreateAsync(newUser, registerModel.Password);

            if (!registerResult.Succeeded)
            {
                foreach (var error in registerResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(registerModel);
            }

            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("All", "Contacts");
            }

            LoginViewModel loginModel = new LoginViewModel();

            return View(loginModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }

            ApplicationUser currentUser = await this.userManager.FindByNameAsync(loginModel.UserName);

            if (currentUser != null)
            {
                var loginResult = await this.signInManager.PasswordSignInAsync(currentUser, loginModel.Password, false, false);

                if (loginResult.Succeeded)
                {
                    return RedirectToAction("All", "Contacts");
                }
            }

            ModelState.AddModelError("", "Login failure!");

            return View(loginModel);
        }

        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
