using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using RaceCompetitionAPI.Data;
using RaceCompetitionAPI.DTOs;
using RaceCompetitionAPI.Models;

namespace RaceCompetitionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthController(
            ApplicationDbContext context,
            IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            string email = request.Email.Trim().ToLowerInvariant();
            string role = request.Role.Trim();

            if (role != "Participant" && role != "Organiser")
            {
                return BadRequest(new
                {
                    message = "Invalid role."
                });
            }

            bool emailExists = await _context.Users
                .AnyAsync(u => u.email.ToLower() == email);

            if (emailExists)
            {
                return Conflict(new
                {
                    message = "Email is already registered."
                });
            }

            if (role == "Participant" &&
                (string.IsNullOrWhiteSpace(request.Name) ||
                 string.IsNullOrWhiteSpace(request.Surname)))
            {
                return BadRequest(new
                {
                    message = "Name and surname are required for participants."
                });
            }

            var user = new User
            {
                email = email,
                role = role
            };

            user.passwordHash = _passwordHasher.HashPassword(
                user, request.Password);

            _context.Users.Add(user);

            // Save first so the database generates the userID.
            await _context.SaveChangesAsync();

            // Create the participant profile linked to this user.
            if (role == "Participant")
            {
                var participant = new Participant
                {
                    userID = user.userId,
                    name = request.Name.Trim(),
                    surname = request.Surname.Trim()
                };

                _context.Participants.Add(participant);
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                message = "Registration successful.",
                userID = user.userId,
                role = user.role
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            string email = request.Email.Trim().ToLowerInvariant();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.email == email);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.passwordHash,
                request.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.userId.ToString()),
        new Claim(ClaimTypes.Email, user.email),
        new Claim(ClaimTypes.Role, user.role)
    };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return Ok(new
            {
                message = "Login successful.",
                userId = user.userId,
                email = user.email,
                role = user.role
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok(new
            {
                message = "Logout successful."
            });
        }


    }
}