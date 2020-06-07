using System.ComponentModel.DataAnnotations;

namespace Ord.WebApi.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        [Required]
        public int ItemId { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal SubTotal{ get; set; }
    }
}
