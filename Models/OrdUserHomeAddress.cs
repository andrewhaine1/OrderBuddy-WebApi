using Ord.WebApi.Models.Shared;

namespace Ord.WebApi.Models
{
    public class OrdUserHomeAddress : Address
    {
        public string HouseNumber { get; set; }

        public string UnitNumber { get; set; }

        public string ComplexName { get; set; }
    }
}
