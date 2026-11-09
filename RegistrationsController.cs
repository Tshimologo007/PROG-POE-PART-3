
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceCompetitionAPI.Data;
using RaceCompetitionAPI.Models;

namespace RaceCompetitionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RegistrationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/registrations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Registration>>> GetRegistrations()
        {
            var registrations = await _context.Registrations.ToListAsync();

            return Ok(registrations);
        }

        // GET: api/registrations/participant/1
        [HttpGet("participant/{userID}")]
        public async Task<ActionResult<IEnumerable<Registration>>> GetParticipantRegistrations(int userID)
        {
            var registrations = await _context.Registrations
                .Where(r => r.userID == userID)
                .ToListAsync();

            return Ok(registrations);
        }

        // POST: api/registrations
        [HttpPost]
        public async Task<IActionResult> RegisterForEvent([FromBody] Registration registration)
        {
            // Check whether the participant exists
            var participant = await _context.Users
                .FirstOrDefaultAsync(u => u.userId == registration.userID);

            if (participant == null)
            {
                return NotFound("Participant not found.");
            }

            // Check whether the event exists
            var raceEvent = await _context.Events
                .FirstOrDefaultAsync(e => e.eventID == registration.eventID);

            if (raceEvent == null)
            {
                return NotFound("Event not found.");
            }

            // Prevent duplicate registrations
            bool alreadyRegistered = await _context.Registrations
                .AnyAsync(r => r.userID == registration.userID
                            && r.eventID == registration.eventID);

            if (alreadyRegistered)
            {
                return BadRequest("You are already registered for this event.");
            }

            registration.registrationID = 0;
            registration.registration_Date = DateTime.Now;

            _context.Registrations.Add(registration);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Registration successful.",
                registration.registrationID
            });
        }
    }
}

