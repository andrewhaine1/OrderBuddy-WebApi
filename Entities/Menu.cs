using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ord.WebApi.Entities
{
    public class Menu : BaseEntity
    {
        [ForeignKey("RestaurantId")]
        public Restaurant Restaurant { get; set; }

        [Required]
        public int RestaurantId { get; set; }

        [Required]
        public string Name { get; set; }

        public IEnumerable<MenuCategory> MenuCategories { get; set; }
    }
}
