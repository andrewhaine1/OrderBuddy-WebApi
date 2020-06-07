using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Ord.WebApi.Mappers.Menu;
using Ord.WebApi.Services.Data.Menus;

namespace Ord.WebApi.Controllers
{
    [ApiController]
    public class MenusController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly IMenuMapper _menuMapper;

        public MenusController(IMenuService menuService,
            IMenuMapper menuMapper)
        {
            _menuService = menuService;
            _menuMapper = menuMapper;
        }

        [HttpGet]
        [Route("api/restaurants/{restaurantId}/menus", Name = "GetMenus")]
        public async Task<IActionResult> GetMenus(int restaurantId)
        {
            // Get menus for restaurant
            var menuEnts = await _menuService.GetMenusAsync(restaurantId);

            // Map menu entities to models
            var menuMods = _menuMapper.MenuEntitiesToModels(menuEnts);

            return Ok(menuMods);
        }

        [HttpGet]
        [Route("api/restaurants/{restaurantId}/menubyid/{menuId}", 
            Name = "GetMenuForRestaurantById")]
        public async Task<IActionResult> GetMenu(int menuId)
        {
            // Get menus for restaurant
            var menuEnt = await _menuService.GetMenuAsync(menuId);

            if (menuEnt == null)
                return NotFound(new { Error = $"A menu with the Id '{menuId}' could not be found." });

            // Map menu entities to models
            var menuMod = _menuMapper.MenuEntityToModel(menuEnt);

            return Ok(menuMod);
        }

        [HttpGet]
        [Route("api/menus/{menuId}", Name = "GetMenuById")]
        public async Task<IActionResult> GetMenuById(int menuId)
        {
            // Get menus for restaurant
            var menuEnt = await _menuService.GetMenuAsync(menuId);

            if (menuEnt == null)
                return NotFound(new { Error = $"A menu with the Id '{menuId}' could not be found." });

            // Map menu entities to models
            var menuMod = _menuMapper.MenuEntityToModel(menuEnt);

            return Ok(menuMod);
        }

        [HttpGet]
        [Route("api/restaurants/{restaurantId}/menubyname/{menuName}",
            Name = "GetMenuByName")]
        public async Task<IActionResult> GetMenu(string menuName)
        {
            // Get menus for restaurant
            var menuEnt = await _menuService.GetMenuAsync(menuName);

            if (menuEnt == null)
                return NotFound(new { Error = $"A menu with the name '{menuName}' could not be found." });

            // Map menu entities to models
            var menuMod = _menuMapper.MenuEntityToModel(menuEnt);

            return Ok(menuMod);
        }

        [AllowAnonymous] // Should not be anonymous as menu can only be added by authenticated user/owner and admin.
        [HttpPost]
        [Route("api/restaurants/{restaurantId}/menus", Name = "AddMenu")]
        public async Task<IActionResult> AddMenu(int restaurantId,
            [FromBody] Models.Menu menu)
        {
            if (menu == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            // Check if another menu exists with specified name.
            if (await _menuService.MenuExistsAsync(menu.Name))
                return Conflict(new
                {
                    Error = $"A menu with the name '{menu.Name}' already exists."
                });

            // Get restaurant Id from the route template as opposed to from the consumer.
            menu.RestaurantId = restaurantId;

            // Map restaurant model to entity.
            var menuEnt = _menuMapper.MenuModelToEntity(menu);

            _menuService.AddMenu(menuEnt);

            if (!await _menuService.SaveChangesAsync())
                throw new Exception($"Could not save menu '{menu.Name}'.");

            // Map newly saved restaurant back to model.
            menu = _menuMapper.MenuEntityToModel(menuEnt);

            return CreatedAtRoute("GetMenuById",
                new { menuId = menu.Id },
                menu);
        }

        [AllowAnonymous] // Should not be anonymous as menu can only be added by authenticated user/owner and admin.
        [HttpPut]
        [Route("api/restaurants/{restaurantId}/menus/{menuId}", Name = "PutUpdateMenu")]
        public async Task<IActionResult> UpdateMenu(int menuId,
            [FromBody] Models.Menu menu)
        {
            if (menu == null)
                return BadRequest();

            var menuEnt = await _menuService.GetMenuAsync(menuId);
            if (menuEnt == null)
                return NotFound(new { Error = $"A menu with the ID '{menuId}' could not be found." });

            TryValidateModel(menu);

            // Add validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            _menuMapper.MapMenuModelToEntity(menu, menuEnt);

            _menuService.UpdateMenu(menuEnt);

            if (!await _menuService.SaveChangesAsync())
                throw new Exception($"Error updating menu '{menuEnt.Name}'.");

            return NoContent();
        }

        [AllowAnonymous] // Should not be anonymous as menu can only be added by authenticated user/owner and admin.
        [HttpPatch]
        [Route("api/restaurants/{restaurantId}/menus/{menuId}", Name = "PatchUpdateMenu")]
        public async Task<IActionResult> UpdateMenu(int menuId,
            [FromBody] JsonPatchDocument<Models.Menu> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var menuEnt = await _menuService.GetMenuAsync(menuId);
            if (menuEnt == null)
                return NotFound(new { Error = $"A menu with the ID '{menuId}' could not be found." });

            var menuMod = _menuMapper.MenuEntityToModel(menuEnt);

            patchDoc.ApplyTo(menuMod);

            TryValidateModel(menuMod);

            // Validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            _menuMapper.MapMenuModelToEntity(menuMod, menuEnt);

            _menuService.UpdateMenu(menuEnt);

            if (!await _menuService.SaveChangesAsync())
                throw new Exception($"Error updating menu '{menuEnt.Name}'.");

            return NoContent();
        }

        [AllowAnonymous] // Should not be anonymous as menu can only be added by authenticated user/owner and admin.
        [HttpPatch]
        [Route("api/restaurants/{restaurantId}/menus/{menuId}", Name = "DeleteMenu")]
        public async Task<IActionResult> DeleteMenu(int menuId)
        {
            var restaurantEnt = await _menuService.GetMenuAsync(menuId);
            if (restaurantEnt == null)
                return NotFound(new { Error = $"A menu with the ID '{menuId}' could not be found." });

            _menuService.DeleteMenu(restaurantEnt);

            if (!await _menuService.SaveChangesAsync())
                throw new Exception($"Error deleting menu '{restaurantEnt.Name}'.");

            return NoContent();
        }
    }
}