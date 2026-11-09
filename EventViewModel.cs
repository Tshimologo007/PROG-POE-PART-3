namespace RaceCompetitionMVC.Models
{
    public class EventViewModel
    {
        public int eventID { get; set; }
        public int organiserID { get; set; }
        public int townID { get; set; }
        public string event_Name { get; set; } 
        public string description { get; set; } 
        public DateTime event_Date { get; set; }
        public string event_Type { get; set; }
        public string venue { get; set; }
        public string race_Distance { get; set; }
        public string? bannerImage { get; set; }
        public IFormFile? bannerFile { get; set; }


    }
}
