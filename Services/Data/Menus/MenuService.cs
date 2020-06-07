using Microsoft.EntityFrameworkCore;
using Ord.WebApi.Entities;
using Ord.WebApi.Helpers.Paging;
using Ord.WebApi.Services.Data.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ord.WebApi.Services.Data.Menus
{
    public class MenuService : IMenuService
    {
        private readonly IDataRepository<Menu> _menuRepo;
        private readonly IDataRepository<MenuCategory> _catRepo;
        private readonly IDataRepository<MenuItem> _itemRepo;

        public MenuService(IDataRepository<Menu> menuRepo, 
            IDataRepository<MenuCategory> categoryRepository,
            IDataRepository<MenuItem> itemRepository)
        {
            _menuRepo = menuRepo;
            _catRepo = categoryRepository;
            _itemRepo = itemRepository;
        }

        /* ------------------------------ Menus --------------------------------------- */
        public async Task<IEnumerable<Menu>> GetMenusAsync(int restaurantId)
            => await _menuRepo.EntityDbSet
            .Where(m => m.RestaurantId == restaurantId)
            .ToListAsync();

        public async Task<Menu> GetMenuAsync(int menuId)
            => await _menuRepo.GetEntityAsync(menuId);

        public async Task<Menu> GetMenuAsync(string menuName)
            => await _menuRepo.EntityDbSet
            .Where(m => m.Name == menuName)
            .FirstOrDefaultAsync();

        public async Task<bool> MenuExistsAsync(string menuName)
            => await _menuRepo.EntityDbSet.AnyAsync(m => m.Name == menuName);

        public void AddMenu(Menu menu)
            => _menuRepo.AddEntity(menu);

        public void UpdateMenu(Menu menu)
            => _menuRepo.UpdateEntity(menu);

        public void DeleteMenu(Menu menu) 
            => _menuRepo.DeleteEntity(menu);


        /* ------------------------------ Menu Categories ----------------------------- */
        public async Task<PagedList<MenuCategory>> GetMenuCategoriesAsync(int menuId,
            ResourceParameters resourceParameters)
            => await PagedList<MenuCategory>.CreateAsync(_catRepo.EntityDbSet
                .Where(c => c.MenuId == menuId)
                .Include(c => c.MenuItems),
                resourceParameters.PageNumber,
                resourceParameters.PageSize);

        public async Task<MenuCategory> GetMenuCategoryAsync(int itemId)
            => await _catRepo.GetEntityAsync(itemId);

        public async Task<MenuCategory> GetMenuCategoryAsync(string itemName)
            => await _catRepo.GetDbSet()
            .Where(c => c.Name == itemName)
            .FirstOrDefaultAsync();

        public async Task<bool> MenuCategoryExistsAsync(int menuId, string name)
            => await _catRepo.EntityDbSet
            .Where(mc => mc.MenuId == menuId)
            .AnyAsync(m => m.Name == name);

        public void AddMenuCategory(MenuCategory menuCategory)
            => _catRepo.AddEntity(menuCategory);

        public void UpdateCategory(MenuCategory menuCategory)
            => _catRepo.UpdateEntity(menuCategory);

        public void DeleteCategory(MenuCategory menuCategory)
            => _catRepo.DeleteEntity(menuCategory);


        /* ------------------------------ Menu Items ----------------------------- */
        public async Task<PagedList<MenuItem>> GetMenuItemsAsync(int categoryId, 
            ResourceParameters resourceParameters)
            => await PagedList<MenuItem>.CreateAsync(_itemRepo.EntityDbSet
                .Where(i => i.CategoryId == categoryId),
                resourceParameters.PageNumber,
                resourceParameters.PageSize); 

        public async Task<MenuItem> GetMenuItemAsync(int itemId)
            => await _itemRepo.GetEntityAsync(itemId);

        public async Task<MenuItem> GetMenuItemAsync(string itemName)
            => await _itemRepo.GetDbSet()
            .Where(i => i.Name == itemName)
            .FirstOrDefaultAsync();

        public async Task<bool> MenuItemExistsAsync(int categoryId, string itemName)
            => await _itemRepo.EntityDbSet
            .Where(mi => mi.CategoryId == categoryId)
            .AnyAsync(i => i.Name == itemName);

        public void AddMenuItem(MenuItem menuItem)
            => _itemRepo.AddEntity(menuItem);

        public void UpdateMenuItem(MenuItem menuItem)
            => _itemRepo.UpdateEntity(menuItem);

        public void DeleteMenuItem(MenuItem menuItem)
            => _itemRepo.DeleteEntity(menuItem);

        public async Task<bool> SaveChangesAsync()
            => await _menuRepo.SaveChangesAsync();
    }
}
