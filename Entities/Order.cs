using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ord.WebApi.Entities
{
    public class Order : BaseEntity
    {   
        [ForeignKey("OrdUserId")]
        public OrdUser OrdUser { get; set; }

        [Required]
        public int OrdUserId { get; set; }

        [ForeignKey("RestaurantId")]
        public Restaurant Restaurant { get; set; }

        [Required]
        public int RestaurantId { get; set; }

	public string StoreOrderNumber { get; set; }

        public OrderInfo OrderInfo { get; set; }

        public IEnumerable<OrderItem> OrderItems { get; set; }
    }
}
