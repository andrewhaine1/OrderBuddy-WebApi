using Ord.WebApi.Entities.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ord.WebApi.Entities
{
    public class OrderInfo : BaseEntity
    {
        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public System.DateTime DateTimePlaced { get; set; }
            = System.DateTime.Now;

        public System.DateTime DateTimeAccepted { get; set; }

        public System.DateTime DateTimeReady { get; set; }

        public System.DateTime DateTimeCompleted { get; set; }

        public System.DateTime DateTimeCancelled { get; set; }

        [Required]
        public decimal OrderTotal { get; set; }

        public OrderStatus OrderStatus { get; set; }
            = OrderStatus.Placed;

        [ForeignKey("RestaurantPaymentMethodId")]
        public RestaurantPaymentMethod RestaurantPaymentMethod { get; set; }

        [Required]
        public int RestaurantPaymentMethodId { get; set; }

        [ForeignKey("RestaurantCollectionTypeId")]
        public RestaurantCollectionType CollectionType { get; set; }

        [Required]
        public int RestaurantCollectionTypeId { get; set; }

        public bool IsPaid { get; set; } = false;

        public bool IsCancelled { get; set; } = false;
    }
}
