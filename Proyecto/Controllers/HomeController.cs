using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proyecto.Models;
using Proyecto.Models.Enums;

namespace Proyecto.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;

        public HomeController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var username = HttpContext.User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "Login");
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                await HttpContext.SignOutAsync();
                return RedirectToAction("Index", "Login");
            }

            if (user.UserType == UserType.Admin)
            {
                return RedirectToAction("Admin", "Home");
            }

            return RedirectToAction("UserHome", "Home");
        }

        public async Task<IActionResult> Admin()
        {
            var username = HttpContext.User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "Login");
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                await HttpContext.SignOutAsync();
                return RedirectToAction("Index", "Login");
            }

            if (user.UserType != UserType.Admin)
            {
                return RedirectToAction("UserHome", "Home");
            }

            return View();
        }

        public async Task<IActionResult> UserHome()
        {
            var username = HttpContext.User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "Login");
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                await HttpContext.SignOutAsync();
                return RedirectToAction("Index", "Login");
            }

            if (user.UserType != UserType.User &&
                user.UserType != UserType.Guest)
            {
                return RedirectToAction("Admin", "Home");
            }

            return View();
        }
    }
}