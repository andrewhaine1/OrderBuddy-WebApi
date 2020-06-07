using System.ComponentModel.DataAnnotations;

namespace Ord.WebApi.Models.Shared
{
    public class Address
    {
        [Required]
        public int OrdUserId { get; set; }

        [Required]
        public string StreetAddress { get; set; }

        public string Suburb { get; set; }

        [Required]
        public string City { get; set; }

        public string Province { get; set; }

        public string Country { get; set; }
    }
}
