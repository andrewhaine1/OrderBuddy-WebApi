using Microsoft.EntityFrameworkCore;
using Ord.WebApi.Entities;
using Ord.WebApi.Helpers.Paging;
using Ord.WebApi.Services.Data.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ord.WebApi.Services.Data.Restaurants
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IDataRepository<Restaurant> _restRepo;

	public DbSet<Restaurant> Restaurants { get { return _restRepo.EntityDbSet; } }

        public RestaurantService(IDataRepository<Restaurant> restRepo)
        {
            _restRepo = restRepo;
        }

        public async Task<PagedList<Restaurant>> GetRestaurantsForSuburbAsync(string suburbName,
            RestaurantResourceParameters restaurantParameters)
        {
            var restaurants = _restRepo.EntityDbSet
                .Where(r => r.Suburb == suburbName)
                .Where(r => r.IsActive)
                .Include(r => r.CollectionTypes)
                .ThenInclude(c => c.CollectionType)
                .Include(r => r.PaymentMethods)
                .ThenInclude(p => p.PaymentMethod);

            return await PagedList<Restaurant>.CreateAsync(restaurants,
                  restaurantParameters.PageNumber,
                  restaurantParameters.PageSize);
        }

        public async Task<PagedList<Restaurant>> GetRestaurantsForCityAsync(string cityName,
            ResourceParameters resourceParameters)
        { 
            return await PagedList<Restaurant>.CreateAsync(_restRepo.EntityDbSet
                .Where(r => r.City == cityName)
                .Where(r => r.IsActive)
                .Include(r => r.CollectionTypes)
                .ThenInclude(c => c.CollectionType)
                .Include(r => r.PaymentMethods)
                .ThenInclude(p => p.PaymentMethod),
                resourceParameters.PageNumber,
                resourceParameters.PageSize);
        }

        public async Task<IEnumerable<Restaurant>> GetRestaurantsForUserAsync(int userId)
            => await _restRepo.EntityDbSet
            .Where(r => r.OrdUserId == userId)
            .Where(r => r.IsActive)
            .Include(r => r.CollectionTypes)
            .ThenInclude(c => c.CollectionType)
            .Include(r => r.PaymentMethods)
            .ThenInclude(p => p.PaymentMethod)
            .ToListAsync();

        public async Task<Restaurant> GetRestaurantAsync(int restaurantId)
            => await _restRepo.EntityDbSet
            .Include(r => r.CollectionTypes)
            .ThenInclude(c => c.CollectionType)
            .Include(r => r.PaymentMethods)
            .ThenInclude(p => p.PaymentMethod)
            .Where(r => r.Id == restaurantId)
            .FirstOrDefaultAsync();

        public async Task<Restaurant> GetRestaurantAsync(string restaurantName)
            => await _restRepo.EntityDbSet
            .Include(r => r.CollectionTypes)
            .ThenInclude(c => c.CollectionType)
            .Include(r => r.PaymentMethods)
            .ThenInclude(p => p.PaymentMethod)
            .Where(r => r.Name == restaurantName)
            .FirstOrDefaultAsync();

        public async Task<bool> RestaurantNameExistsAsync(string restaurantName)
            => await _restRepo.EntityDbSet.AnyAsync(r => r.Name == restaurantName);

        public void AddRestaurant(Restaurant restaurant)
            => _restRepo.AddEntity(restaurant);

        public void UpdateRestaurant(Restaurant restaurant)
            => _restRepo.UpdateEntity(restaurant);

        public void DeleteRestaurant(Restaurant restaurant)
            => _restRepo.DeleteEntity(restaurant);

        public async Task<bool> SaveChangesAsync()
            => await _restRepo.SaveChangesAsync();
    }
}
