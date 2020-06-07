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
    [Route("api/categories")]
    [ApiController]
    public class MenuCategoriesController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly IMenuMapper _menuMapper;

        public MenuCategoriesController(IMenuService menuService,
            IMenuMapper menuMapper)
        {
            _menuService = menuService;
            _menuMapper = menuMapper;
        }

        [HttpGet("formenu/{menuId}", Name = "GetMenuCategories")]
        public async Task<IActionResult> GetMenuCategories(int menuId,
            [FromQuery] MenuResourceParameters menuParams)
        {
            // Get menus for restaurant
            var menuCatEnts = await _menuService.GetMenuCategoriesAsync(menuId,
                menuParams);

            // Map menu entities to models
            var menuCatModels = _menuMapper.MenuCategoryEntitiesToModels(menuCatEnts);

            return Ok(menuCatModels);
        }

        [HttpGet("{categoryId}", Name = "GetMenuCategoryById")]
        public async Task<IActionResult> GetMenuCategoryById(int categoryId)
        {
            // Get menus for restaurant
            var menuCatEnt = await _menuService.GetMenuCategoryAsync(categoryId);

            if (menuCatEnt == null)
                return NotFound(new { Error = $"A category with the Id '{categoryId}' could not be found." });

            // Map menu entities to models
            var menuCatMod = _menuMapper.MenuCategoryEntityToModel(menuCatEnt);

            return Ok(menuCatMod);
        }

        [HttpGet("menucategorybyname/{menuCategoryName}", Name = "GetMenuCategoryByName")]
        public async Task<IActionResult> GetMenuCategoryByName(string menuCategoryName)
        {
            // Get menus for restaurant
            var menuCatEnt = await _menuService.GetMenuCategoryAsync(menuCategoryName);

            if (menuCatEnt == null)
                return NotFound(new { Error = $"A category with the name " +
                    $"'{menuCategoryName}' could not be found." });

            // Map menu entities to models
            var menuCatMod = _menuMapper.MenuCategoryEntityToModel(menuCatEnt);

            return Ok(menuCatMod);
        }

        [AllowAnonymous] // Should not be anonymous as menu category can only be added by authenticated user/owner and admin.
        [HttpPost(Name = "AddMenuCategory")]
        public async Task<IActionResult> AddMenuCategory([FromBody]
            Models.MenuCategory menuCategory)
        {
            if (menuCategory == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            // Check if another menu category exists with specified name.
            if (await _menuService.MenuCategoryExistsAsync(menuCategory.MenuId, menuCategory.Name))
                return Conflict(new
                {
                    Error = $"A category with the name '{menuCategory.Name}' already exists."
                });

            // Map category model to entity.
            var menuCatEnt = _menuMapper.MenuCategoryModelToEntity(menuCategory);

            _menuService.AddMenuCategory(menuCatEnt);

            if (!await _menuService.SaveChangesAsync())
                throw new Exception($"Could not save category '{menuCategory.Name}'.");

            // Map newly saved restaurant back to model.
            menuCategory = _menuMapper.MenuCategoryEntityToModel(menuCatEnt);

            return CreatedAtRoute("GetMenuCategoryById",
                new { categoryId = menuCatEnt.Id },
                menuCategory);
        }

        [AllowAnonymous] // Should not be anonymous as menu category can only be added by authenticated user/owner and admin.
        [HttpPut("{menuCategoryId}", Name = "PutUpdateMenuCategory")]
        public async Task<IActionResult> UpdateMenuCategory(int menuCategoryId,
            [FromBody] Models.MenuCategory menuCatMod)
        {
            if (menuCatMod == null)
                return BadRequest();

            var menuCatEnt = await _menuService.GetMenuCategoryAsync(menuCategoryId);
            if (menuCatEnt == null)
                return NotFound(new
                {
                    Error = $"A menu category with the ID '{menuCategoryId}'" +
                    $" could not be found."
                });

            TryValidateModel(menuCatMod);

            // Add validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            _menuMapper.MapMenuCategoryModelToEntity(menuCatMod, menuCatEnt);

            _menuService.UpdateCategory(menuCatEnt);

            if (!await _menuService.SaveChangesAsync())
                throw new Exception($"Error updating menu category '{menuCatEnt.Name}'.");

            return NoContent();
        }

        [AllowAnonymous] // Should not be anonymous as menu category can only be added by authenticated user/owner and admin.
        [HttpPatch("{menuCategoryId}", Name = "PatchUpdateMenuCategory")]
        public async Task<IActionResult> UpdateMenu(int menuCategoryId,
            [FromBody] JsonPatchDocument<Models.MenuCategory> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var menuCatEnt = await _menuService.GetMenuCategoryAsync(menuCategoryId);
            if (menuCatEnt == null)
                return NotFound(new
                {
                    Error = $"A menu category with the ID '{menuCategoryId}' " +
                    $"could not be found."
                });

            var menuCatMod = _menuMapper.MenuCategoryEntityToModel(menuCatEnt);

            patchDoc.ApplyTo(menuCatMod);

            TryValidateModel(menuCatMod);

            // Validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            _menuMapper.MapMenuCategoryModelToEntity(menuCatMod, menuCatEnt);

            _menuService.UpdateCategory(menuCatEnt);

            if (!await _menuService.SaveChangesAsync())
                throw new Exception($"Error updating menu category '{menuCatEnt.Name}'.");

            return NoContent();
        }

        [AllowAnonymous] // Should not be anonymous as menu category can only be added by authenticated user/owner and admin.
        [HttpDelete("{menuCategoryId}", Name = "DeleteMenuCategory")]
        public async Task<IActionResult> DeleteMenu(int menuCategoryId)
        {
            var menuCatEnt = await _menuService.GetMenuCategoryAsync(menuCategoryId);
            if (menuCatEnt == null)
                return NotFound(new
                {
                    Error = $"A menu category with the ID '{menuCategoryId}' " +
                    $"could not be found."
                });

            _menuService.DeleteCategory(menuCatEnt);

            if (!await _menuService.SaveChangesAsync())
                throw new Exception($"Error deleting menu category '{menuCatEnt.Name}'.");

            return NoContent();
        }
    }
}