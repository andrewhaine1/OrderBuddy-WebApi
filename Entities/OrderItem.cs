using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ord.WebApi.Entities
{
    public class OrderItem : BaseEntity
    {
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("ItemId")]
        public MenuItem Item { get; set; }

        [Required]
        public int ItemId { get; set; }

        public int Quantity { get; set; } 

        public decimal SubTotal { get; set; }
    }
}
