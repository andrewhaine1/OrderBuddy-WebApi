using Ord.WebApi.Entities.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ord.WebApi.Entities
{
    public class Restaurant : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [ForeignKey("OrdUserId")]
        public OrdUser OrdUser { get; set; }

        [Required]
        public int OrdUserId { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }

        public string ShopNumber { get; set; }

        public string ComplexName { get; set; }

        public string ShoppingCenterName { get; set; }

        [Required]
        public string StreetAddress { get; set; }

        public string Suburb { get; set; }

        [Required]
        public string City { get; set; }

        public string Province { get; set; }

        [Required]
        public string Country { get; set; }

        [ForeignKey("ServiceAreaId")]
        public ServiceArea ServiceArea { get; set; }

        [Required]
        public int ServiceAreaId { get; set; }

        public string RestaurantImagePath { get; set; } 
            = "~/Media/Shared/Images/fast_food_meal.jpg";

        public IEnumerable<Menu> Menus { get; set; }

        public IEnumerable<RestaurantPaymentMethod> PaymentMethods { get; set; }

        public IEnumerable<RestaurantCollectionType> CollectionTypes { get; set; }

        public bool IsHalal { get; set; } = false;

        public bool IsActive { get; set; }

        public bool IsSuspended { get; set; }
    }
}
