
namespace RaceCompetitionAPI.Models
{
    public class Participant
    {
        public int participantID { get; set; }
        public int userID { get; set; }
        public string name { get; set; }
        public string surname { get; set; }

        public string? profileImage { get; set; }
    }
}

