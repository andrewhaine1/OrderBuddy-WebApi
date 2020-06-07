using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Ord.WebApi.Helpers.Paging;
using Ord.WebApi.Mappers.Menu;
using Ord.WebApi.Services.Data.Menus;

namespace Ord.WebApi.Controllers
{
    [Route("api/menuitems")]
    [ApiController]
    public class MenuItemsController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly IMenuMapper _menuMapper;

        public MenuItemsController(IMenuService menuService,
            IMenuMapper menuMapper)
        {
            _menuService = menuService;
            _menuMapper = menuMapper;
        }

        [HttpGet("{categoryId}", Name = "GetMenuItems")]
        public async Task<IActionResult> GetMenuItems(int categoryId,
            [FromQuery] MenuResourceParameters menuParams)
        {
            // Get menu items for menu category
            var menuItemEnts = await _menuService.GetMenuItemsAsync(categoryId,
                menuParams);

            // Map menu item entities to models
            var menuItemModels = _menuMapper.MenuItemEntitiesToModels(menuItemEnts);

            return Ok(menuItemModels);
        }

        [HttpGet("byid/{menuItemId}", Name = "GetMenuItemById")]
        public async Task<IActionResult> GetMenuItem(int menuItemId)
        {
            // Get menus for restaurant
            var menuItemEnt = await _menuService.GetMenuItemAsync(menuItemId);

            if (menuItemEnt == null)
                return NotFound(new
                {
                    Error = $"A menu item with the Id '{menuItemId}' " +
                    $"could not be found."
                });

            // Map menu entities to models
            var menuItemMod = _menuMapper.MenuItemEntityToModel(menuItemEnt);

            return Ok(menuItemMod);
        }

        [HttpGet("byname/{menuItemName}", Name = "GetMenuItemByName")]
        public async Task<IActionResult> GetMenuItem(string menuItemName)
        {
            // Get menus for restaurant
            var menuItemEnt = await _menuService.GetMenuItemAsync(menuItemName);

            if (menuItemEnt == null)
                return NotFound(new
                {
                    Error = $"A menu item with the Id '{menuItemName}' " +
                    $"could not be found."
                });

            // Map menu entities to models
            var menuItemMod = _menuMapper.MenuItemEntityToModel(menuItemEnt);

            return Ok(menuItemMod);
        }

        [AllowAnonymous] // Should not be anonymous as menu category can only be added by authenticated user/owner and admin.
        [HttpPost(Name = "AddMenuItem")]
        public async Task<IActionResult> AddMenuItem([FromBody] Models.MenuItem menuItem)
        {
            if (menuItem == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            // Check if another menu category exists with specified name.
            if (await _menuService.MenuItemExistsAsync(menuItem.CategoryId, menuItem.Name))
                return Conflict(new
                {
                    Error = $"A menu item with the name '{menuItem.Name}' already exists."
                });

            // Map category model to entity.
            var menuItemEnt = _menuMapper.MenuItemModelToEntity(menuItem);

            _menuService.AddMenuItem(menuItemEnt);

            if (!await _menuService.SaveChangesAsync())
                throw new Exception($"Could not save menu item '{menuItem.Name}'.");

            // Map newly saved restaurant back to model.
            menuItem = _menuMapper.MenuItemEntityToModel(menuItemEnt);

            return CreatedAtRoute("GetMenuItemById",
                new { menuItemId = menuItemEnt.Id },
                menuItem);
        }

        [AllowAnonymous] // Should not be anonymous as menu category can only be added by authenticated user/owner and admin.
        [HttpPut("{menuItemId}", Name = "PutUpdateMenuItem")]
        public async Task<IActionResult> UpdateMenuItem(int menuItemId,
            [FromBody] Models.MenuItem menuItemMod)
        {
            if (menuItemMod == null)
                return BadRequest();

            var menuItemEnt = await _menuService.GetMenuItemAsync(menuItemId);
            if (menuItemEnt == null)
                return NotFound(new
                {
                    Error = $"A menu item with the ID '{menuItemId}'" +
                    $" could not be found."
                });

            TryValidateModel(menuItemMod);

            // Add validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            _menuMapper.MapMenuItemModelToEntity(menuItemMod, menuItemEnt);

            _menuService.UpdateMenuItem(menuItemEnt);

            if (!await _menuService.SaveChangesAsync())
                throw new Exception($"Error updating menu item '{menuItemEnt.Name}'.");

            return NoContent();
        }

        [AllowAnonymous] // Should not be anonymous as menu category can only be added by authenticated user/owner and admin.
        [HttpPatch("{menuCategoryId}", Name = "PatchUpdateMenuItem")]
        public async Task<IActionResult> UpdateMenuItem(int menuItemId,
            [FromBody] JsonPatchDocument<Models.MenuItem> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var menuItemEnt = await _menuService.GetMenuItemAsync(menuItemId);
            if (menuItemEnt == null)
                return NotFound(new
                {
                    Error = $"A menu item with the ID '{menuItemId}' " +
                    $"could not be found."
                });

            var menuItemMod = _menuMapper.MenuItemEntityToModel(menuItemEnt);

            patchDoc.ApplyTo(menuItemMod);

            TryValidateModel(menuItemMod);

            // Validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            _menuMapper.MapMenuItemModelToEntity(menuItemMod, menuItemEnt);

            _menuService.UpdateMenuItem(menuItemEnt);

            if (!await _menuService.SaveChangesAsync())
                throw new Exception($"Error updating menu item '{menuItemEnt.Name}'.");

            return NoContent();
        }

        [AllowAnonymous] // Should not be anonymous as menu category can only be added by authenticated user/owner and admin.
        [HttpDelete("{menuItemId}", Name = "DeleteMenuItem")]
        public async Task<IActionResult> DeleteMenu(int menuItemId)
        {
            var menuItemEnt = await _menuService.GetMenuItemAsync(menuItemId);
            if (menuItemEnt == null)
                return NotFound(new
                {
                    Error = $"A menu item with the ID '{menuItemId}' " +
                    $"could not be found."
                });

            _menuService.DeleteMenuItem(menuItemEnt);

            if (!await _menuService.SaveChangesAsync())
                throw new Exception($"Error deleting menu item '{menuItemEnt.Name}'.");

            return NoContent();
        }
    }
}