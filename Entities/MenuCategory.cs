using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ord.WebApi.Entities
{
    public class MenuCategory : BaseEntity
    {
        [ForeignKey("MenuId")]
        public Menu Menu { get; set; }

        [Required]
        public int MenuId { get; set; }

        public string Name { get; set; }

        public IEnumerable<MenuItem> MenuItems { get; set; }
    }
}
