
namespace RaceCompetitionMVC.Models
{
    public class ManageEventCategoriesViewModel
    {
        public EventViewModel Event { get; set; } = new();

        public List<CategoryViewModel> AvailableCategories { get; set; } = new();

        public List<EventCategoryViewModel> AssignedCategories { get; set; } = new();
    }
}

