using System.ComponentModel.DataAnnotations;

namespace Ord.WebApi.Models
{
    public class Menu
    {
        [Required]
        public int RestaurantId { get; set; }

        [Required]
        public string Name { get; set; }
        public object Id { get; internal set; }
    }
}
