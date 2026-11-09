
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceCompetitionAPI.Models;
using RaceCompetitionAPI.Data;

namespace RaceCompetitionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrolmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnrolmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Find a participant record using their login UserID
        [HttpGet("participant-record/{userID}")]
        public async Task<IActionResult> GetParticipantRecord(int userID)
        {
            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.userID == userID);

            if (participant == null)
            {
                return NotFound("Participant record not found.");
            }

            return Ok(participant);
        }

        // Get all enrolments for a participant
        [HttpGet("participant/{participantID}")]
        public async Task<IActionResult> GetParticipantEnrolments(int participantID)
        {
            var enrolments = await _context.Enrolments
                .Where(e => e.participantID == participantID)
                .ToListAsync();

            return Ok(enrolments);
        }

        // Get all participants enrolled in a specific event
        [HttpGet("event/{eventID}")]
        public async Task<IActionResult> GetEventEnrolments(int eventID)
        {
            var eventExists = await _context.Events
                .AnyAsync(e => e.eventID == eventID);

            if (!eventExists)
            {
                return NotFound("Event not found.");
            }

            var enrolments = await (
                from en in _context.Enrolments
                join p in _context.Participants
                    on en.participantID equals p.participantID
                join ec in _context.EventCategories
                    on en.eventCategoryID equals ec.eventCategoryID
                join c in _context.Categories
                    on ec.categoryID equals c.categoryID
                where ec.eventID == eventID
                select new
                {
                    en.enrolmentID,
                    en.participantID,
                    p.name,
                    p.surname,
                    en.eventCategoryID,
                    c.category_Name,
                    en.enrolment_Date
                }
            ).ToListAsync();

            return Ok(enrolments);
        }

        // Create an enrolment
        [HttpPost]
        public async Task<IActionResult> CreateEnrolment(
            [FromBody] Enrolment enrolment)
        {
            var participant = await _context.Participants
                .FindAsync(enrolment.participantID);

            if (participant == null)
            {
                return NotFound("Participant not found.");
            }

            var eventCategory = await _context.EventCategories
                .FindAsync(enrolment.eventCategoryID);

            if (eventCategory == null)
            {
                return NotFound("Event category not found.");
            }

            // Prevent duplicate enrolments in the same event
            var alreadyEnrolled = await _context.Enrolments
                .AnyAsync(e =>
                    e.participantID == enrolment.participantID &&
                    e.eventCategoryID == enrolment.eventCategoryID);

            if (alreadyEnrolled)
            {
                return BadRequest("You are already enrolled in this category.");
            }

            enrolment.enrolmentID = 0;
            enrolment.enrolment_Date = DateTime.Now;

            _context.Enrolments.Add(enrolment);

            await _context.SaveChangesAsync();

            return Ok(enrolment);
        }
    }
}
