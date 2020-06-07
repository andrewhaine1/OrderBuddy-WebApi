using System.ComponentModel.DataAnnotations;

namespace Ord.WebApi.Entities.Shared
{
    public class PaymentMethod : BaseEntity
    {
        [Required]
        public string Name { get; set; }
    }
}