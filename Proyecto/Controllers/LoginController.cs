using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proyecto.Models;
using Proyecto.Models.Enums;

namespace Proyecto.Controllers
{
    public class Login : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public Login(
            SignInManager<User> signInManager,
            UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        private async Task CreateAdminIfNotExists()
        {
            var user = await _userManager.FindByNameAsync("admin");

            if (user == null)
            {
                user = new User
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.Now,
                    UserType = UserType.Admin,
                    IsActive = true
                };

                await _userManager.CreateAsync(user, "admin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                ModelState.AddModelError("username", "El usuario es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("password", "La contraseña es obligatoria.");
            }

            if (!ModelState.IsValid)
            {
                return View("Index");
            }

            if (username == "admin" && password == "admin")
            {
                await CreateAdminIfNotExists();
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                ModelState.AddModelError("", "El usuario o la contraseña son incorrectos.");
                return View("Index");
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError("", "El usuario se encuentra desactivado.");
                return View("Index");
            }

            var result = await _signInManager.PasswordSignInAsync(
                username,
                password,
                false,
                false
            );

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "El usuario se encuentra bloqueado.");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "No tienes permitido iniciar sesión.");
                }
                else
                {
                    ModelState.AddModelError("", "El usuario o la contraseña son incorrectos.");
                }

                return View("Index");
            }

            switch (user.UserType)
            {
                case UserType.Admin:
                    return RedirectToAction("Admin", "Home");

                case UserType.User:
                case UserType.Guest:
                    return RedirectToAction("UserHome", "Home");

                default:
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError("", "Tipo de usuario no válido.");
                    return View("Index");
            }
        }

        [Authorize]
        [HttpPost("/Auth/Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
