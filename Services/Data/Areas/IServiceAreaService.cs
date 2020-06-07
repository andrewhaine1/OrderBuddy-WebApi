using Ord.WebApi.Entities;
using Ord.WebApi.Helpers.Paging;
using System.Threading.Tasks;

namespace Ord.WebApi.Services.Data.Areas
{
    public interface IServiceAreaService
    {
        /* ------------------------------ Service Areas ----------------------------- */
        Task<PagedList<ServiceArea>> GetServiceAreasAsync(ResourceParameters resourceParameters);

        Task<PagedList<ServiceArea>> GetServiceAreasAsync(string city,
            ResourceParameters resourceParameters);

        Task<ServiceArea> GetServiceAreaAsync(int serviceAreaId);

        Task<ServiceArea> GetServiceAreaAsync(string suburb);

        Task<bool> SuburbIsInService(string suburb);

        Task<bool> CityIsInService(string city);

        void AddServiceArea(ServiceArea serviceArea);

        void UpdateServiceArea(ServiceArea serviceArea);

        void DeleteServiceArea(ServiceArea serviceArea);
    }
}
