using Ord.WebApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace Ord.WebApi.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        //public string Price { get; set; }
        public decimal Price { get; set; }
    }
}
