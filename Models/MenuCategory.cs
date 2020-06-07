using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ord.WebApi.Models
{
    public class MenuCategory
    {
        public int Id { get; set; }

        [Required]
        public int MenuId { get; set; }

        public string Name { get; set; }

        public IEnumerable<MenuItem> MenuItems { get; set; }
    }
}
