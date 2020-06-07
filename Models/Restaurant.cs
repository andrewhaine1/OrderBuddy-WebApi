using Ord.WebApi.Models.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ord.WebApi.Models
{
    public class Restaurant
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

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

        [Required]
        public int ServiceAreaId { get; set; }

        public string RestaurantImagePath { get; set; }

        public IEnumerable<RestaurantCollectionType> CollectionTypes { get; set; }

        public IEnumerable<RestaurantPaymentMethod> PaymentMethods { get; set; }

        public bool IsHalal { get; set; }

        public bool IsActive { get; set; }

        public bool IsSuspended { get; set; }
    }
}
