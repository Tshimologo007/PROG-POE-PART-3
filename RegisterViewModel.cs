using System.ComponentModel.DataAnnotations;

namespace RaceCompetitionMVC.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } = "Participant";

        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";

    }
}
