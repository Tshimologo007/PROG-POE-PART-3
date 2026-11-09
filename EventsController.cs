    using global::RaceCompetitionMVC.Models;
    using Microsoft.AspNetCore.Mvc;
    using RaceCompetitionMVC.Models;

    namespace RaceCompetitionMVC.Controllers
    {
        public class EventsController : Controller
        {
            private readonly IHttpClientFactory _httpClientFactory;

            public EventsController(IHttpClientFactory httpClientFactory)
            {
                _httpClientFactory = httpClientFactory;
            }

            public async Task<IActionResult> Index()
            {
                var client = _httpClientFactory
                    .CreateClient("RaceCompetitionAPI");

                var events = await client.GetFromJsonAsync<List<EventViewModel>>(
                    "api/events");

                return View(events ?? new List<EventViewModel>());
            }
        }
    }

