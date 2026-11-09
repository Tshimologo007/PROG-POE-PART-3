
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceCompetitionAPI.Data;
using RaceCompetitionAPI.Models;

namespace RaceCompetitionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventCategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EventCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all category assignments
        [HttpGet]
        public async Task<IActionResult> GetEventCategories()
        {
            var items = await _context.EventCategories.ToListAsync();
            return Ok(items);
        }

        // Get categories assigned to a specific event
        [HttpGet("event/{eventID}")]
        public async Task<IActionResult> GetByEvent(int eventID)
        {
            var items = await _context.EventCategories
                .Where(ec => ec.eventID == eventID)
                .ToListAsync();

            return Ok(items);
        }

        // Assign a category to an event
        [HttpPost]
        public async Task<IActionResult> AssignCategory(
            [FromBody] EventCategory eventCategory)
        {
            var eventExists = await _context.Events
                .AnyAsync(e => e.eventID == eventCategory.eventID);

            if (!eventExists)
                return NotFound("Event not found.");

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.categoryID == eventCategory.categoryID);

            if (!categoryExists)
                return NotFound("Category not found.");

            var duplicate = await _context.EventCategories
                .AnyAsync(ec =>
                    ec.eventID == eventCategory.eventID &&
                    ec.categoryID == eventCategory.categoryID);

            if (duplicate)
                return BadRequest("This category is already assigned to the event.");

            eventCategory.eventCategoryID = 0;

            _context.EventCategories.Add(eventCategory);
            await _context.SaveChangesAsync();

            return Ok(eventCategory);
        }

        // Remove a category from an event
        [HttpDelete("{eventCategoryID}")]
        public async Task<IActionResult> RemoveCategory(int eventCategoryID)
        {
            var item = await _context.EventCategories
                .FindAsync(eventCategoryID);

            if (item == null)
                return NotFound();

            _context.EventCategories.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

