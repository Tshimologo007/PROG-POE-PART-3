namespace RaceCompetitionAPI.Models
{
    public class Enrolment
    {
        public int enrolmentID { get; set; }
        public int participantID { get; set; }
        public int eventCategoryID { get; set; }
        public DateTime enrolment_Date { get; set; }
    }
}
