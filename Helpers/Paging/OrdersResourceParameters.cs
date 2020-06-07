namespace Ord.WebApi.Helpers.Paging
{
    public class OrdersResourceParameters : ResourceParameters
    {
        public int RestId { get; set; }          // Owner/Admin only

        public string RestName { get; set; }     // Owner/Admin only

        public int UserId { get; set; }          // User/Admin/Owner of restaurant only

        public string DateRange { get; set; } = "Today";

        public int OrderNumber { get; set; }
    }
}
