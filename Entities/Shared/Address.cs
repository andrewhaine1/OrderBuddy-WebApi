using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ord.WebApi.Entities.Shared
{
    public class Address : BaseEntity
    {
        [ForeignKey("OrdUserId")]
        public OrdUser OrdUser { get; set; }

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
