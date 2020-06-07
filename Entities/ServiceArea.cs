namespace Ord.WebApi.Entities
{
    public class ServiceArea : BaseEntity
    {
        public string Suburb { get; set; }

        public string City { get; set; }

        public string Country { get; set; }
            = "South Africa";
    }
}
