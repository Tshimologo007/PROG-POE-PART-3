using Microsoft.AspNetCore.Mvc;
using RaceCompetitionMVC.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace RaceCompetitionMVC.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ParticipantController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null || role != "Participant")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }


        public async Task<IActionResult> Events()
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null || role != "Participant")
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("RaceCompetitionAPI");

            // Get all events
            var events = await client.GetFromJsonAsync<List<EventViewModel>>(
                "api/events");

            // Get all categories
            var categories = await client.GetFromJsonAsync<List<CategoryViewModel>>(
                "api/category");

            events ??= new List<EventViewModel>();
            categories ??= new List<CategoryViewModel>();

            // Store assigned categories for each event
            var eventCategories =
                new Dictionary<int, List<EventCategoryViewModel>>();

            foreach (var raceEvent in events)
            {
                var assignedCategories =
                    await client.GetFromJsonAsync<List<EventCategoryViewModel>>(
                        $"api/eventcategories/event/{raceEvent.eventID}");

                assignedCategories ??= new List<EventCategoryViewModel>();

                var matchedCategories = new List<EventCategoryViewModel>();

                foreach (var assigned in assignedCategories)
                {
                    var category = categories.FirstOrDefault(
                        c => c.categoryID == assigned.categoryID);

                    if (category != null)
                    {
                        matchedCategories.Add(new EventCategoryViewModel
                        {
                            eventCategoryID = assigned.eventCategoryID,
                            eventID = assigned.eventID,
                            categoryID = assigned.categoryID,
                            category_Name = category.category_Name,
                            description = category.description
                        });
                    }
                }

                eventCategories[raceEvent.eventID] = matchedCategories;
            }

            ViewBag.EventCategories = eventCategories;

            return View(events);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int eventCategoryID)
        {
            // Check that the participant is logged in
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null || role != "Participant")
            {
                return RedirectToAction("Login", "Account");
            }

            if (eventCategoryID <= 0)
            {
                TempData["ErrorMessage"] = "Please select a category.";
                return RedirectToAction("Events");
            }

            var client = _httpClientFactory.CreateClient("RaceCompetitionAPI");

            // Find the participant record using the logged-in user's ID
            var participantResponse = await client.GetAsync(
                $"api/enrolments/participant-record/{userID.Value}");

            if (!participantResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Participant profile not found.";
                return RedirectToAction("Events");
            }

            var participantData =
                await participantResponse.Content.ReadFromJsonAsync<JsonElement>();

            int participantID = 0;

            if (participantData.TryGetProperty("participantID", out var idProperty))
            {
                participantID = idProperty.GetInt32();
            }
            else if (participantData.TryGetProperty("participantId", out idProperty))
            {
                participantID = idProperty.GetInt32();
            }

            if (participantID <= 0)
            {
                TempData["ErrorMessage"] = "Invalid participant record.";
                return RedirectToAction("Events");
            }

            // Prepare the enrolment information
            var enrolment = new
            {
                participantID = participantID,
                eventCategoryID = eventCategoryID
            };

            // Send the enrolment to the API
            var response = await client.PostAsJsonAsync(
                "api/enrolments", enrolment);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] =
                    "You have successfully enrolled in this event!";
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                TempData["ErrorMessage"] = errorMessage;
            }

            return RedirectToAction("Events");
        }



        public async Task<IActionResult> MyRegistrations()
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null || role != "Participant")
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("RaceCompetitionAPI");

            // Get this participant's registrations
            var registrations = await client.GetFromJsonAsync<List<RegistrationViewModel>>(
                $"api/registrations/participant/{userID.Value}");

            // Get all available events
            var events = await client.GetFromJsonAsync<List<EventViewModel>>(
                "api/events");

            registrations ??= new List<RegistrationViewModel>();
            events ??= new List<EventViewModel>();

            // Match each registration with its event details
            var myRegistrations = from registration in registrations
                                  join raceEvent in events
                                  on registration.eventID equals raceEvent.eventID
                                  select new MyRegistrationViewModel
                                  {
                                      registrationID = registration.registrationID,
                                      eventID = raceEvent.eventID,
                                      registration_Date = registration.registration_Date,
                                      event_Name = raceEvent.event_Name,
                                      description = raceEvent.description,
                                      event_Date = raceEvent.event_Date,
                                      event_Type = raceEvent.event_Type,
                                      venue = raceEvent.venue,
                                      race_Distance = raceEvent.race_Distance
                                  };

            return View(myRegistrations.ToList());
        }

        public async Task<IActionResult> MyEnrolments()
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null || role != "Participant")
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("RaceCompetitionAPI");

            // Find the participant profile linked to the logged-in user.
            var participantResponse = await client.GetAsync(
                $"api/enrolments/participant-record/{userID.Value}");

            if (!participantResponse.IsSuccessStatusCode)
            {
                return View(new List<EnrolmentViewModel>());
            }

            var participant = await participantResponse.Content
                .ReadFromJsonAsync<JsonElement>();

            int participantID = participant.GetProperty("participantID").GetInt32();

            // Retrieve the participant's enrolments.
            var enrolments = await client.GetFromJsonAsync<List<EnrolmentViewModel>>(
                $"api/enrolments/participant/{participantID}");

            // Retrieve events, categories and event-category assignments.
            var events = await client.GetFromJsonAsync<List<EventViewModel>>(
                "api/events");

            var categories = await client.GetFromJsonAsync<List<CategoryViewModel>>(
                "api/category");

            var eventCategories = await client.GetFromJsonAsync<List<EventCategoryViewModel>>(
                "api/eventcategories");

            enrolments ??= new List<EnrolmentViewModel>();
            events ??= new List<EventViewModel>();
            categories ??= new List<CategoryViewModel>();
            eventCategories ??= new List<EventCategoryViewModel>();

            // Match each enrolment to its event and selected category.
            foreach (var enrolment in enrolments)
            {
                var assignment = eventCategories.FirstOrDefault(
                    ec => ec.eventCategoryID == enrolment.eventCategoryID);

                if (assignment != null)
                {
                    var raceEvent = events.FirstOrDefault(
                        e => e.eventID == assignment.eventID);

                    var category = categories.FirstOrDefault(
                        c => c.categoryID == assignment.categoryID);

                    if (raceEvent != null)
                    {
                        enrolment.event_Name = raceEvent.event_Name;
                        enrolment.event_Date = raceEvent.event_Date;
                        enrolment.venue = raceEvent.venue;
                        enrolment.race_Distance = raceEvent.race_Distance;
                    }

                    if (category != null)
                    {
                        enrolment.category_Name = category.category_Name;
                    }
                }
            }

            return View(enrolments);
        }


        public async Task<IActionResult> MyResults()
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null || role != "Participant")
                return RedirectToAction("Login", "Account");

            var client = _httpClientFactory.CreateClient("RaceCompetitionAPI");

            // Get the participant's profile
            var participantResponse = await client.GetAsync(
                $"api/enrolments/participant-record/{userID.Value}");

            if (!participantResponse.IsSuccessStatusCode)
                return View(new List<JsonElement>());

            var participant = await participantResponse.Content
                .ReadFromJsonAsync<JsonElement>();

            if (!participant.TryGetProperty("participantID", out var participantIDProperty))
                return View(new List<JsonElement>());

            int participantID = participantIDProperty.GetInt32();

            // Get this participant's results and enrolments
            var results = await client.GetFromJsonAsync<List<JsonElement>>(
                $"api/participant/{participantID}/results");

            var enrolments = await client.GetFromJsonAsync<List<EnrolmentViewModel>>(
                $"api/enrolments/participant/{participantID}");

            // Get event and category information
            var events = await client.GetFromJsonAsync<List<EventViewModel>>(
                "api/events");

            var eventCategories = await client.GetFromJsonAsync<List<EventCategoryViewModel>>(
                "api/eventcategories");

            var categories = await client.GetFromJsonAsync<List<CategoryViewModel>>(
                "api/category");

            results ??= new List<JsonElement>();
            enrolments ??= new List<EnrolmentViewModel>();
            events ??= new List<EventViewModel>();
            eventCategories ??= new List<EventCategoryViewModel>();
            categories ??= new List<CategoryViewModel>();

            // Combine result, enrolment, event and category information
            var myResults = new List<JsonElement>();

            foreach (var result in results)
            {
                if (!result.TryGetProperty("enrolmentID", out var enrolmentIDProperty))
                    continue;

                int enrolmentID = enrolmentIDProperty.GetInt32();

                var enrolment = enrolments.FirstOrDefault(
                    e => e.enrolmentID == enrolmentID);

                if (enrolment == null)
                    continue;

                var assignment = eventCategories.FirstOrDefault(
                    ec => ec.eventCategoryID == enrolment.eventCategoryID);

                if (assignment == null)
                    continue;

                var raceEvent = events.FirstOrDefault(
                    e => e.eventID == assignment.eventID);

                var category = categories.FirstOrDefault(
                    c => c.categoryID == assignment.categoryID);

                if (raceEvent == null)
                    continue;

                var resultDetails = new
                {
                    event_Name = raceEvent.event_Name,
                    event_Date = raceEvent.event_Date,
                    category_Name = category?.category_Name ?? "Unknown",
                    finish_time = result.TryGetProperty("finish_time", out var finish)
                        ? finish.ToString() : "Not available",
                    position = result.TryGetProperty("position", out var position)
                        ? position.ToString() : "Not available",
                    completionStatus = result.TryGetProperty("completionStatus", out var status)
                        ? status.ToString() : "Not available",
                    distance = result.TryGetProperty("distance", out var distance)
                        ? distance.ToString() : ""
                };

                myResults.Add(JsonSerializer.SerializeToElement(resultDetails));
            }

            return View(myResults);
        }


        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null || role != "Participant")
                return RedirectToAction("Login", "Account");

            var client = _httpClientFactory.CreateClient("RaceCompetitionAPI");

            var response = await client.GetAsync(
                $"api/enrolments/participant-record/{userID.Value}");

            if (!response.IsSuccessStatusCode)
                return NotFound("Participant profile not found.");

            var participant = await response.Content.ReadFromJsonAsync<JsonElement>();

            ViewBag.Name = participant.GetProperty("name").GetString();
            ViewBag.Surname = participant.GetProperty("surname").GetString();

            ViewBag.ProfileImage = participant.TryGetProperty(
                "profileImage", out var imageProperty)
                ? imageProperty.GetString()
                : null;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfilePicture(IFormFile profileFile)
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null || role != "Participant")
                return RedirectToAction("Login", "Account");

            if (profileFile == null || profileFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a picture.";
                return RedirectToAction("Profile");
            }

            var client = _httpClientFactory.CreateClient("RaceCompetitionAPI");

            try
            {
                // Find participant record.
                var participantResponse = await client.GetAsync(
                    $"api/enrolments/participant-record/{userID.Value}");

                if (!participantResponse.IsSuccessStatusCode)
                    return NotFound("Participant profile not found.");

                var participant = await participantResponse.Content
                    .ReadFromJsonAsync<JsonElement>();

                int participantID = participant.GetProperty("participantID").GetInt32();

                // Upload image to Azure using the existing API endpoint.
                using var form = new MultipartFormDataContent();
                using var stream = profileFile.OpenReadStream();
                using var fileContent = new StreamContent(stream);

                fileContent.Headers.ContentType =
                    new MediaTypeHeaderValue(profileFile.ContentType);

                form.Add(fileContent, "file", profileFile.FileName);
                form.Add(new StringContent("profiles"), "folder");

                var uploadResponse = await client.PostAsync(
                    "api/images/upload", form);

                if (!uploadResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] =
                        await uploadResponse.Content.ReadAsStringAsync();

                    return RedirectToAction("Profile");
                }

                var uploadResult = await uploadResponse.Content
                    .ReadFromJsonAsync<JsonElement>();

                string? blobName = uploadResult.GetProperty("blobName").GetString();

                // Save Azure blob name against participant record.
                var updateResponse = await client.PutAsJsonAsync(
                    $"api/participants/{participantID}/profile-image",
                    new { profileImage = blobName });

                if (!updateResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] =
                        await updateResponse.Content.ReadAsStringAsync();

                    return RedirectToAction("Profile");
                }

                TempData["SuccessMessage"] = "Profile picture uploaded successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Profile");
        }


    }
}

