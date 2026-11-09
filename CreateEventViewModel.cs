
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RaceCompetitionMVC.Models
{
    public class CreateEventViewModel
    {
        [Required]
        public int townID { get; set; }

        [Required]
        public string event_Name { get; set; } = "";

        [Required]
        public string description { get; set; } = "";

        [Required]
        public DateTime event_Date { get; set; }

        [Required]
        public string event_Type { get; set; } = "";

        [Required]
        public string venue { get; set; } = "";

        [Required]
        public string race_Distance { get; set; } = "";

        public IFormFile? bannerFile { get; set; }
    }
}

