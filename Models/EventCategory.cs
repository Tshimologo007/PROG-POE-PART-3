using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceCompetitionAPI.Models
{
    public class EventCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int eventCategoryID { get; set; }

        public int eventID { get; set; }

        public int categoryID { get; set; }
    }
}
