
namespace RaceCompetitionMVC.Models
{
    public class EventCategoryViewModel
    {
        public int eventCategoryID { get; set; }
        public int eventID { get; set; }
        public int categoryID { get; set; }

        public string category_Name { get; set; } = "";
        public string description { get; set; } = "";
    }
}

