namespace Ord.WebApi.Helpers.Paging
{
    public class ResourceParameters
    {
        const int maxPageSize = 250;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 50;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        public string SearchQuery { get; set; }

        public string OrderBy { get; set; }

        public string Fields { get; set; }
    }
}
