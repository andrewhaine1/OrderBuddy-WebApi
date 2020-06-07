using Ord.WebApi.Models.Shared;
using System.ComponentModel.DataAnnotations;

namespace Ord.WebApi.Models
{
    public class OrderInfo
    {
        public long Id { get; set; }

        public int OrderId { get; set; }

        public System.DateTime DateTimePlaced { get; set; }
            = System.DateTime.Now;

        public System.DateTime DateTimeAccepted { get; set; }

        public System.DateTime DateTimeReady { get; set; }

        public System.DateTime DateTimeCompleted { get; set; }

        public System.DateTime DateTimeCancelled { get; set; }

        [Required]
        public decimal OrderTotal { get; set; }

        public string OrderStatus { get; set; }

        public int RestaurantPaymentMethodId { get; set; }

        public int RestaurantCollectionTypeId { get; set; }

        public bool IsPaid { get; set; }

        public bool IsCancelled { get; set; } = false;
    }
}
