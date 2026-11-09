namespace RaceCompetitionMVC.Models
{
    public class EnrolmentViewModel
    {
        public int enrolmentID { get; set; }
        public int participantID { get; set; }
        public int eventCategoryID { get; set; }
        public DateTime enrolment_Date { get; set; }

        // Details retrieved from the related event and category
        public string event_Name { get; set; } = "";
        public string category_Name { get; set; } = "";
        public DateTime event_Date { get; set; }
        public string venue { get; set; } = "";
        public string race_Distance { get; set; } = "";
    }
}
