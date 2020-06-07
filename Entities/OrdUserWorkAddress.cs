using Ord.WebApi.Entities.Shared;

namespace Ord.WebApi.Entities
{
    public class OrdUserWorkAddress : Address
    {
        public string OfficeParkName { get; set; }

        public string BuildingName { get; set; }

        public string FloorNumber { get; set; }

        public string DepartmentName { get; set; }
    }
}
