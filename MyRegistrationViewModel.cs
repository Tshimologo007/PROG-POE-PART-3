
namespace RaceCompetitionMVC.Models
{
    public class MyRegistrationViewModel
    {
        public int registrationID { get; set; }

        public int eventID { get; set; }

        public DateTime registration_Date { get; set; }

        public string event_Name { get; set; } = "";

        public string description { get; set; } = "";

        public DateTime event_Date { get; set; }

        public string event_Type { get; set; } = "";

        public string venue { get; set; } = "";

        public string race_Distance { get; set; } = "";
    }
}
