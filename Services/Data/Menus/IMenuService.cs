using Ord.WebApi.Entities;
using Ord.WebApi.Helpers.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ord.WebApi.Services.Data.Menus
{
    public interface IMenuService
    {
        /* ------------------------------ Menus --------------------------------------- */
        Task<IEnumerable<Menu>> GetMenusAsync(int restaurantId);

        Task<Menu> GetMenuAsync(int menuId);

        Task<Menu> GetMenuAsync(string menuName);

        Task<bool> MenuExistsAsync(string menuName);

        void AddMenu(Menu menu);

        void UpdateMenu(Menu menu);

        void DeleteMenu(Menu menu);

        /* ------------------------------ Menu Categories ----------------------------- */
        Task<PagedList<MenuCategory>> GetMenuCategoriesAsync(int menuId, ResourceParameters resourceParameters);

        Task<MenuCategory> GetMenuCategoryAsync(int itemId);

        Task<MenuCategory> GetMenuCategoryAsync(string itemName);

        Task<bool> MenuCategoryExistsAsync(int menuId, string name);

        void AddMenuCategory(MenuCategory menuCategory);

        void UpdateCategory(MenuCategory menuCategory);

        void DeleteCategory(MenuCategory menuCategory);


        /* ------------------------------ Menu Items ----------------------------- */
        Task<PagedList<MenuItem>> GetMenuItemsAsync(int categoryId, ResourceParameters resourceParameters);

        Task<MenuItem> GetMenuItemAsync(int itemId);

        Task<MenuItem> GetMenuItemAsync(string itemName);

        Task<bool> MenuItemExistsAsync(int categoryId, string itemName);

        void AddMenuItem(MenuItem menuItem);

        void UpdateMenuItem(MenuItem menuItem);

        void DeleteMenuItem(MenuItem menuItem);

        Task<bool> SaveChangesAsync();
    }
}
