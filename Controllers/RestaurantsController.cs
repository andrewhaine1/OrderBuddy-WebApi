using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ord.WebApi.Entities;
using Ord.WebApi.Helpers.Constants;
using Ord.WebApi.Helpers.Paging;
using Ord.WebApi.Mappers.Restaurant;
using Ord.WebApi.Services.Data.Areas;
using Ord.WebApi.Services.Data.Restaurants;

namespace Ord.WebApi.Controllers
{
    [Authorize]
    [Route("api/restaurants")]
    public class RestaurantsController : Controller
    {
        private readonly IRestaurantService _restService;
        private readonly IRestaurantMapper _restMapper;
        private readonly IServiceAreaService _saService;
        //private readonly IHttpContextAccessor _httpContext;

        public RestaurantsController(IRestaurantService restService,
            IRestaurantMapper restaurantMapper,
            IServiceAreaService serviceAreaService)
        {
            _restService = restService;
            _restMapper = restaurantMapper;
            _saService = serviceAreaService;
        }

        [AllowAnonymous]
        [HttpGet(Name = "GetRestaurants")]
        public async Task<IActionResult> GetRestaurants(RestaurantResourceParameters restaurantParams)
        {
            // If no suburb and no city has been specified, get all restaurants for Johannesburg
            // which will be the default city for all users. If no suburb and no city was specified
            // I assume that geolocation failed so the default will be loaded unless the user specifies
            // a different suburb or city.

            // Might have to get suburb and city from user so if suburb is not in service areas 
            // then city can be searched and if city is not in suburb areas, load default.    
	    if (!string.IsNullOrEmpty(restaurantParams.Suburb) &&
                !string.IsNullOrEmpty(restaurantParams.City))
                return BadRequest(new
                {
                    Error = "Cannot request restaurants by suburb and city together. " +
                    "Please request either suburb or city separately."
                });

            PagedList<Restaurant> restaurants = null;
            if (!string.IsNullOrEmpty(restaurantParams.Suburb))
                restaurants = await _restService.GetRestaurantsForSuburbAsync(restaurantParams.Suburb,
                        restaurantParams);

            if (!string.IsNullOrEmpty(restaurantParams.City))
                restaurants = await _restService.GetRestaurantsForCityAsync(restaurantParams.City,
                        restaurantParams);

            if (restaurants == null)
                restaurants = new PagedList<Restaurant>();

            var paginationMetadata = new
            {
                totalCount = restaurants.TotalCount,
                pageSize = restaurants.PageSize,
                currentPage = restaurants.CurrentPage,
                totalPages = restaurants.TotalPages
            };

            var models = _restMapper.RestaurantEntitiesToModels(restaurants);

            return Ok(new { Values = models, PagingInfo = paginationMetadata });
        }

        [AllowAnonymous]
        [HttpGet("foruser/{userId}", Name = "GetRestaurantsForUser")]
        public async Task<IActionResult> GetRestaurants(int userId)
        {
            var restaurants = await _restService.GetRestaurantsForUserAsync(userId);

            var models = _restMapper.RestaurantEntitiesToModels(restaurants);

            return Ok(models);
        }

        [AllowAnonymous]
        [HttpGet("{restaurantId}", Name = "GetRestaurant")]
        public async Task<IActionResult> GetRestaurant(int restaurantId)
        {
            var restaurant = await _restService.GetRestaurantAsync(restaurantId);

            if (restaurant == null)
                return BadRequest(new { Error = $"A restaurant with the Id '{restaurantId}' " +
                    $"could not be found." });

            var model = _restMapper.RestaurantEntityToModel(restaurant);

            return Ok(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("servicearea", Name = "GetServiceAreaRestaurants")]
        public async Task<IActionResult> GetServiceAreaRestaurants(ServiceArea serviceArea,
            RestaurantResourceParameters restaurantParams)
        {
            // Check that serviceArea object is not null, if so return error 400.
            if (string.IsNullOrEmpty(serviceArea.Suburb) &&
                string.IsNullOrEmpty(serviceArea.City))
                return BadRequest(new
                {
                    Error = "Service Area suburb and city were not provided."
                });

            // Check if suburb is in service areas, if not, move onto city.
            if (await _saService.SuburbIsInService(serviceArea.Suburb))
            {
                var restaurants = await _restService.GetRestaurantsForSuburbAsync(serviceArea.Suburb,
                    restaurantParams);

                SetRestaurantsPaginationMetaData(restaurants);

                var models = _restMapper.RestaurantEntitiesToModels(restaurants);
                return Ok(models);
            }

            // Check if city is in service areas, if not, redirect to default city.
            if (await _saService.CityIsInService(serviceArea.City))
            {
                var restaurants = await _restService.GetRestaurantsForCityAsync(serviceArea.City,
                    restaurantParams);

                SetRestaurantsPaginationMetaData(restaurants);

                var models = _restMapper.RestaurantEntitiesToModels(restaurants);
                return Ok(models);
            }

            return RedirectToAction("GetDefaultCityRestaurants");
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("defaultcity", Name = "GetDefaultCityRestaurants")]
        public async Task<IActionResult> GetDefaultCityRestaurants(RestaurantResourceParameters restaurantParams)
        {
            var restaurants = await _restService.GetRestaurantsForCityAsync(OrdConstants.DefaultCity,
                        restaurantParams);

            SetRestaurantsPaginationMetaData(restaurants);

            var models = _restMapper.RestaurantEntitiesToModels(restaurants);

            return Ok(models);
        }

        // Should not be anonymous as restaurant can only be added by authenticated user/owner and admin.
        [HttpPost(Name = "AddRestaurant")]
        public async Task<IActionResult> AddRestaurant([FromBody] Models.Restaurant restaurant)
        {
            if (restaurant == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            // Check if another restaurant exists with specified name.
            if (await _restService.RestaurantNameExistsAsync(restaurant.Name))
                return Conflict(new
                {
                    Error = $"A restaurant with the name '{restaurant.Name}' already exists."
                });

            // Map restaurant model to entity.
            var restaurantEntity = _restMapper.RestaurantModelToEntity(restaurant);
            restaurantEntity.IsActive = false;
            restaurantEntity.IsSuspended = false;

            _restService.AddRestaurant(restaurantEntity);

            if (!await _restService.SaveChangesAsync())
                throw new Exception($"Could not save restaurant '{restaurant.Name}'.");

            // Map newly saved restaurant back to model.
            restaurant = _restMapper.RestaurantEntityToModel(restaurantEntity);

            return CreatedAtRoute("GetRestaurants",
                new { restaurantId = restaurant.Id },
                restaurant);
        }

        // Should not be anonymous as restaurant can only be added by authenticated user/owner and admin.
        [HttpPut("{id}", Name = "PutUpdateRestaurant")]
        public async Task<IActionResult> UpdateRestaurant(int id,
            [FromBody] Models.Restaurant restaurant)
        {
            if (restaurant == null)
                return BadRequest();

            var restaurantEnt = await _restService.GetRestaurantAsync(id);
            if (restaurantEnt == null)
                return NotFound(new { Error = $"A restaurant with the ID '{id}' could not be found." });

            TryValidateModel(restaurant);

            // Add validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            _restMapper.MapRestaurantModelToEntity(restaurant, restaurantEnt);

            _restService.UpdateRestaurant(restaurantEnt);

            if (!await _restService.SaveChangesAsync())
                throw new Exception($"Error updating restaurant '{restaurantEnt.Name}'.");

            return NoContent();
        }

        // Should not be anonymous as restaurant can only be added by authenticated user/owner and admin.
        [HttpPatch("{id}", Name = "PatchUpdateRestarant")]
        public async Task<IActionResult> UpdateRestaurant(int id,
            [FromBody] JsonPatchDocument<Models.Restaurant> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var restaurantEnt = await _restService.GetRestaurantAsync(id);
            if (restaurantEnt == null)
                return NotFound(new { Error = $"A restaurant with the ID '{id}' could not be found." });

            var restaurantMod = _restMapper.RestaurantEntityToModel(restaurantEnt);

            patchDoc.ApplyTo(restaurantMod);

            TryValidateModel(restaurantMod);

            // Validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            _restMapper.MapRestaurantModelToEntity(restaurantMod, restaurantEnt);

            _restService.UpdateRestaurant(restaurantEnt);

            if (!await _restService.SaveChangesAsync())
                throw new Exception($"Error updating restaurant '{restaurantEnt.Name}'.");

            return NoContent();
        }

        // Should not be anonymous as restaurant can only be added by authenticated user/owner and admin.
        [HttpDelete("{id}", Name = "DeleteRestaurant")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var restaurantEnt = await _restService.GetRestaurantAsync(id);
            if (restaurantEnt == null)
                return NotFound(new { Error = $"A restaurant with the ID '{id}' could not be found." });

            _restService.DeleteRestaurant(restaurantEnt);

            if (!await _restService.SaveChangesAsync())
                throw new Exception($"Error deleting restaurant '{restaurantEnt.Name}'.");

            return NoContent();
        }

        private void SetRestaurantsPaginationMetaData(PagedList<Restaurant> restaurants)
        {
            var paginationMetaData = new
            {
                totalCount = restaurants.TotalCount,
                pageSize = restaurants.PageSize,
                currentPage = restaurants.CurrentPage,
                totalPages = restaurants.TotalPages,
                //previousPage = previousPageLink,
                //nextPage = nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetaData));
        }

        // Should not be anonymous as restaurant can only be added by authenticated user/owner and admin.
        [HttpPatch("activaterestaurant/{id}", Name = "ActivateRestaurant")]
        public async Task<IActionResult> ActivateRestaurant(int id,
            [FromBody] JsonPatchDocument<Models.Restaurant> patchDoc)
        {
            return Forbid();

            if (patchDoc == null)
                return BadRequest();

            var restaurantEnt = await _restService.GetRestaurantAsync(id);
            if (restaurantEnt == null)
                return NotFound(new { Error = $"A restaurant with the ID '{id}' could not be found." });

            var restaurantMod = _restMapper.RestaurantEntityToModel(restaurantEnt);

            patchDoc.ApplyTo(restaurantMod);

            TryValidateModel(restaurantMod);

            // Validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            _restMapper.MapRestaurantModelToEntity(restaurantMod, restaurantEnt);

            _restService.UpdateRestaurant(restaurantEnt);

            if (!await _restService.SaveChangesAsync())
                throw new Exception($"Error activating restaurant '{restaurantEnt.Name}'.");

            return NoContent();
        }


        [HttpGet]
        [Route("{restaurantId}/issuspended", Name = "RestaurantIsSuspended")]
        public async Task<IActionResult> RestaurantIsSuspended(int restaurantId)
        {
            var restaurant = await _restService.GetRestaurantAsync(restaurantId);

            if (restaurant == null)
                return BadRequest(new
                {
                    Error = $"A restaurant with the Id '{restaurantId}' " +
                    $"could not be found."
                });

            return Ok(new { restaurant.IsSuspended });
        }
    }
}
