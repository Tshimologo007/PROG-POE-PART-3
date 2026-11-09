
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RaceCompetitionMVC.Models;

namespace RaceCompetitionMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // REGISTER - Display page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // REGISTER - Submit form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory
                .CreateClient("RaceCompetitionAPI");

            var response = await client.PostAsJsonAsync(
                "api/auth/register", new
                {
                    model.Email,
                    model.Password,
                    model.Role
                });

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                ViewBag.ErrorMessage =
                    $"Registration failed ({(int)response.StatusCode}): {error}";

                return View(model);
            }

            ViewBag.SuccessMessage =
                "Registration successful! You can now log in.";

            return View(new RegisterViewModel());
        }

        // LOGIN - Display page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // LOGIN - Submit form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory
                .CreateClient("RaceCompetitionAPI");

            var response = await client.PostAsJsonAsync(
                "api/auth/login", new
                {
                    model.Email,
                    model.Password
                });

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                ViewBag.ErrorMessage =
                    $"Login failed ({(int)response.StatusCode}): {error}";

                return View(model);
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (result == null)
            {
                ViewBag.ErrorMessage = "Unable to process login response.";
                return View(model);
            }

            HttpContext.Session.SetInt32("UserID", result.UserID);

            HttpContext.Session.SetString("Email", result.Email);

            HttpContext.Session.SetString("Role", result.Role);

            // We'll add session storage and role-based redirects next.

            if (result.Role == "Participant")
            {
                return RedirectToAction("Index", "Participant");
            }
            else if (result.Role == "Organiser")
            {
                return RedirectToAction("Index", "Organiser");
            }

            HttpContext.Session.Clear();

            ViewBag.ErrorMessage = "Invalid user role.";
            return View(model);
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account");
        }



    }
}
