
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using RaceCompetitionAPI.Data;

namespace RaceCompetitionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ParticipantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPut("{id}/profile-image")]
        public async Task<IActionResult> UpdateProfileImage(
            int id,
            [FromBody] JsonElement request)
        {
            if (!request.TryGetProperty("profileImage", out var imageProperty))
                return BadRequest("Profile image is required.");

            string? imageName = imageProperty.GetString();

            if (string.IsNullOrWhiteSpace(imageName))
                return BadRequest("Invalid profile image.");

            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.participantID == id);

            if (participant == null)
                return NotFound("Participant not found.");

            participant.profileImage = imageName;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile picture updated successfully.",
                profileImage = participant.profileImage
            });
        }
    }
}

