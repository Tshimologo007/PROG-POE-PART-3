
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceCompetitionAPI.Models
{
    public class Registration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int registrationID { get; set; }

        public int userID { get; set; }

        public int eventID { get; set; }

        public DateTime registration_Date { get; set; } = DateTime.Now;
    }
}

