using Ord.WebApi.Entities.Shared;

namespace Ord.WebApi.Entities
{
    public class OrdUserHomeAddress : Address
    {
        public string HouseNumber { get; set; }

        public string UnitNumber { get; set; }

        public string ComplexName { get; set; }
    }
}
