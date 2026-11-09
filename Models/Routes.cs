namespace RaceCompetitionAPI.Models
{
    public class Routes
    {
        public int RoutesID { get; set; }
        public int eventID { get; set; }
        public string route_Name {get; set;}
        public string route_Description {get; set;}
        public string mapURL {get; set;}
    }
}
