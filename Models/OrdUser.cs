using System.ComponentModel.DataAnnotations;

namespace Ord.WebApi.Models
{
    public class OrdUser
    {
        [Required]
        public int OauthId { get; set; }

        [Required]
        public string FullName { get; set; }

        public string EmailAddress { get; set; }

        public string MobileNumber { get; set; }

        public string DeviceToken { get; set; }

        public bool CanPlaceOrders { get; set; }
    }
}
