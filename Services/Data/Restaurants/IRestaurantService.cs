using Ord.WebApi.Entities;
using Ord.WebApi.Helpers.Paging;
using System.Threading.Tasks;

namespace Ord.WebApi.Services.Data.Restaurants
{
    public interface IRestaurantService
    {
        /* ---------------------------------------------- Restaurants ------------------------------------- */

	Microsoft.EntityFrameworkCore.DbSet<Restaurant> Restaurants { get; }

        Task<PagedList<Restaurant>> GetRestaurantsForSuburbAsync(string suburbName, 
            RestaurantResourceParameters restaurantParameters);

        Task<PagedList<Restaurant>> GetRestaurantsForCityAsync(string cityName, 
            ResourceParameters resourceParameters);

        Task<System.Collections.Generic.IEnumerable<Restaurant>> GetRestaurantsForUserAsync(int userId);

        Task<Restaurant> GetRestaurantAsync(int restaurantId);

        Task<Restaurant> GetRestaurantAsync(string restaurantName);

        Task<bool> RestaurantNameExistsAsync(string restaurantName);

        void AddRestaurant(Restaurant restaurant);

        void UpdateRestaurant(Restaurant restaurant);

        void DeleteRestaurant(Restaurant restaurant);

        Task<bool> SaveChangesAsync();
    }
}
