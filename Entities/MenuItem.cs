using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ord.WebApi.Entities
{
    public class MenuItem : BaseEntity
    {
        [ForeignKey("CategoryId")]
        public MenuCategory Category { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
