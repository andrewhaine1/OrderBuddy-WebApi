using Ord.WebApi.Models.Shared;

namespace Ord.WebApi.Models
{
    public class OrdUserWorkAddress : Address
    {
        public string OfficeParkName { get; set; }
        public string BuildingName { get; set; }
        public string FloorNumber { get; set; }
        public string DepartmentName { get; set; }
    }
}
