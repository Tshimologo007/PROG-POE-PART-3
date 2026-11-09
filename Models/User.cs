namespace RaceCompetitionAPI.Models
{
    public class User
    {
        public int userId { get; set; }
        public string email { get; set; }
        public string passwordHash { get; set; }
        public string role { get; set; }
    }
}
