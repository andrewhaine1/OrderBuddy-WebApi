using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ord.WebApi.Models
{
    public class Order
    {
        public long Id { get; set; }

        [Required]
        public int OrdUserId { get; set; }
	public OrdUserForOrder OrdUser { get; set; }

        [Required]
        public int RestaurantId { get; set; }

	public string StoreOrderNumber { get; set; }

        public OrderInfo OrderInfo { get; set; }

        public IEnumerable<OrderItem> OrderItems { get; set; }
    }
}
