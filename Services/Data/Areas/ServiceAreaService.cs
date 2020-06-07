using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ord.WebApi.Entities;
using Ord.WebApi.Helpers.Paging;
using Ord.WebApi.Services.Data.Shared;

namespace Ord.WebApi.Services.Data.Areas
{
    public class ServiceAreaService : IServiceAreaService
    {
        private readonly IDataRepository<ServiceArea> _serviceAreaRepo;

        public ServiceAreaService(IDataRepository<ServiceArea> serviceAreaRepo)
        {
            _serviceAreaRepo = serviceAreaRepo;
        }

        public async Task<PagedList<ServiceArea>> GetServiceAreasAsync(ResourceParameters resourceParameters)
            => await _serviceAreaRepo.GetEntitesPagedListAsync(resourceParameters);

        public async Task<PagedList<ServiceArea>> GetServiceAreasAsync(string city, ResourceParameters resourceParameters)
            => await PagedList<ServiceArea>.CreateAsync(_serviceAreaRepo.EntityDbSet
            .Where(sa => sa.City == city), 
                resourceParameters.PageNumber, 
                resourceParameters.PageSize);

        public async Task<ServiceArea> GetServiceAreaAsync(int serviceAreaId)
            => await _serviceAreaRepo.GetEntityAsync(serviceAreaId);

        public async Task<ServiceArea> GetServiceAreaAsync(string suburb)
            => await _serviceAreaRepo.EntityDbSet
            .Where(sa => sa.Suburb == suburb)
            .FirstOrDefaultAsync();

        public async Task<bool> SuburbIsInService(string suburb)
            => await _serviceAreaRepo.EntityDbSet
            .AnyAsync(sa => sa.Suburb == suburb);

        public async Task<bool> CityIsInService(string city)
        => await _serviceAreaRepo.EntityDbSet
            .AnyAsync(sa => sa.City == city);

        public void AddServiceArea(ServiceArea serviceArea)
            => _serviceAreaRepo.AddEntity(serviceArea);

        public void UpdateServiceArea(ServiceArea serviceArea)
            => _serviceAreaRepo.UpdateEntity(serviceArea);

        public void DeleteServiceArea(ServiceArea serviceArea)
            => _serviceAreaRepo.DeleteEntity(serviceArea);
    }
}
