namespace RaceCompetitionAPI.Models
{
    public class Result
    {
        public int resultID { get; set; }
        public int enrolmentID { get; set; }
        public string finish_time { get; set; }
        public string distance { get; set; }
        public string completionStatus { get; set; }
        public string position { get; set; }
    }
}
