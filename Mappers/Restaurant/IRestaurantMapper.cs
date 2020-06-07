using Ord.WebApi.Helpers.Paging;

namespace Ord.WebApi.Mappers.Restaurant
{
    public interface IRestaurantMapper
    {
        PagedList<Models.Restaurant> RestaurantEntitiesToModels(PagedList<Entities.Restaurant> restaurants);

        System.Collections.Generic.IEnumerable<Models.Restaurant> RestaurantEntitiesToModels(System.Collections.Generic.IEnumerable<Entities.Restaurant> restaurants);

        Models.Restaurant RestaurantEntityToModel(Entities.Restaurant restaurant);

        Entities.Restaurant RestaurantModelToEntity(Models.Restaurant restaurant);

        void MapRestaurantModelToEntity(Models.Restaurant restaurantMod,
            Entities.Restaurant restaurantEnt);
    }
}
