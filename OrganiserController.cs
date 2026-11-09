using Microsoft.AspNetCore.Mvc;
using RaceCompetitionMVC.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Net.Http.Headers;

namespace RaceCompetitionMVC.Controllers
{
    public class OrganiserController : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public OrganiserController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        public async Task<IActionResult> Events()
        {
            var client = _httpClientFactory
                .CreateClient("RaceCompetitionAPI");

            var events = await client.GetFromJsonAsync<List<EventViewModel>>(
                "api/events");

            return View(events ?? new List<EventViewModel>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEventViewModel model)
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null || role != "Organiser")
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient("RaceCompetitionAPI");

            try
            {
                string? bannerImage = await UploadEventBanner(
                    client, model.bannerFile);

                var newEvent = new
                {
                    organiserID = userID.Value,
                    model.townID,
                    model.event_Name,
                    model.description,
                    model.event_Date,
                    model.event_Type,
                    model.venue,
                    model.race_Distance,
                    bannerImage
                };

                var response = await client.PostAsJsonAsync(
                    "api/events", newEvent);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = await response.Content.ReadAsStringAsync();
                    return View(model);
                }

                return RedirectToAction("Events");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Image upload failed: " + ex.Message;
                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("RaceCompetitionAPI");

            var eventItem = await client.GetFromJsonAsync<EventViewModel>(
                $"api/events/{id}");

            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventViewModel model)
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null || role != "Organiser")
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient("RaceCompetitionAPI");

            try
            {
                // Get the existing event so we preserve its current banner.
                var existingEvent = await client.GetFromJsonAsync<EventViewModel>(
                    $"api/events/{model.eventID}");

                if (existingEvent == null)
                    return NotFound();

                // Keep the existing banner unless a new one is uploaded.
                string? bannerImage = existingEvent.bannerImage;

                if (model.bannerFile != null && model.bannerFile.Length > 0)
                {
                    bannerImage = await UploadEventBanner(client, model.bannerFile);
                }

                // Send event details and banner name to the API.
                var updatedEvent = new
                {
                    model.eventID,
                    model.organiserID,
                    model.townID,
                    model.event_Name,
                    model.description,
                    model.event_Date,
                    model.event_Type,
                    model.venue,
                    model.race_Distance,
                    bannerImage
                };

                var response = await client.PutAsJsonAsync(
                    $"api/events/{model.eventID}",
                    updatedEvent);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = await response.Content.ReadAsStringAsync();
                    model.bannerImage = bannerImage;
                    return View(model);
                }

                return RedirectToAction("Events");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Unable to update event: " + ex.Message;
                return View(model);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("RaceCompetitionAPI");

            var response = await client.DeleteAsync($"api/events/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Unable to delete event.";
            }

            return RedirectToAction("Events");
        }


        [HttpGet]
        public async Task<IActionResult> ManageCategories(int id)
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("RaceCompetitionAPI");

            // Get the selected event
            var eventItem = await client.GetFromJsonAsync<EventViewModel>(
                $"api/events/{id}");

            if (eventItem == null)
            {
                return NotFound();
            }

            // Get all available categories
            var categories = await client.GetFromJsonAsync<List<CategoryViewModel>>(
                "api/category");

            // Get categories already assigned to this event
            var assigned = await client.GetFromJsonAsync<List<EventCategoryViewModel>>(
                $"api/eventcategories/event/{id}");

            var model = new ManageEventCategoriesViewModel
            {
                Event = eventItem,
                AvailableCategories = categories ?? new List<CategoryViewModel>(),
                AssignedCategories = assigned ?? new List<EventCategoryViewModel>()
            };

            // Add category names and descriptions for display
            foreach (var item in model.AssignedCategories)
            {
                var category = model.AvailableCategories
                    .FirstOrDefault(c => c.categoryID == item.categoryID);

                if (category != null)
                {
                    item.category_Name = category.category_Name;
                    item.description = category.description;
                }
            }

            // Only show categories that haven't already been assigned
            model.AvailableCategories = model.AvailableCategories
                .Where(c => !model.AssignedCategories.Any(
                    a => a.categoryID == c.categoryID))
                .ToList();

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignCategory(int eventID, int categoryID)
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("RaceCompetitionAPI");

            var assignment = new
            {
                eventID = eventID,
                categoryID = categoryID
            };

            var response = await client.PostAsJsonAsync(
                "api/eventcategories", assignment);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Unable to assign category. It may already be assigned.";
            }
            else
            {
                TempData["SuccessMessage"] = "Category assigned successfully.";
            }

            return RedirectToAction("ManageCategories", new { id = eventID });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveCategory(int eventCategoryID, int eventID)
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("RaceCompetitionAPI");

            var response = await client.DeleteAsync(
                $"api/eventcategories/{eventCategoryID}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Unable to remove category.";
            }
            else
            {
                TempData["SuccessMessage"] = "Category removed successfully.";
            }

            return RedirectToAction("ManageCategories", new { id = eventID });
        }

        // Display participants enrolled in an event and their results
        [HttpGet]
        public async Task<IActionResult> Results(int id)
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("RaceCompetitionAPI");

            var raceEvent = await client.GetFromJsonAsync<EventViewModel>(
                $"api/events/{id}");

            if (raceEvent == null)
            {
                return NotFound();
            }

            var enrolments = await client.GetFromJsonAsync<List<JsonElement>>(
                $"api/enrolments/event/{id}");

            var results = await client.GetFromJsonAsync<List<JsonElement>>(
                $"api/results/{id}/results");

            ViewBag.Event = raceEvent;
            ViewBag.ExistingResults = results ?? new List<JsonElement>();

            return View(enrolments ?? new List<JsonElement>());
        }


        // Save a participant's result
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveResult(
            int eventID,
            int enrolmentID,
            string finish_time,
            string position,
            string distance,
            string completionStatus)
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("RaceCompetitionAPI");

            var result = new
            {
                enrolmentID = enrolmentID,
                finish_time = finish_time,
                distance = distance,
                completionStatus = completionStatus,
                position = position
            };

            var response = await client.PostAsJsonAsync(
                "api/results", result);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Result saved successfully.";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();

                TempData["ErrorMessage"] = error;
            }

            return RedirectToAction("Results", new { id = eventID });
        }


        private async Task<string?> UploadEventBanner(
            HttpClient client, IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            using var form = new MultipartFormDataContent();

            using var stream = file.OpenReadStream();
            using var fileContent = new StreamContent(stream);

            fileContent.Headers.ContentType =
                new MediaTypeHeaderValue(file.ContentType);

            form.Add(fileContent, "file", file.FileName);
            form.Add(new StringContent("events"), "folder");

            var response = await client.PostAsync(
                "api/images/upload", form);

            if (!response.IsSuccessStatusCode)
                throw new Exception(await response.Content.ReadAsStringAsync());

            var uploadResult = await response.Content
                .ReadFromJsonAsync<JsonElement>();

            return uploadResult.GetProperty("blobName").GetString();
        }


    }
}
