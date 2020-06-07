using System.ComponentModel.DataAnnotations;

namespace Ord.WebApi.Models.Shared
{
    public class RestaurantPaymentMethod
    {
	public int Id { get; set; }	

        [Required]
        public int RestaurantId { get; set; }

        [Required]
        public int PaymentMethodId { get; set; }

        public string Name { get; set; }
    }
}
