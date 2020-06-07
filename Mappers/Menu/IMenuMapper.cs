using System.Collections.Generic;

namespace Ord.WebApi.Mappers.Menu
{
    public interface IMenuMapper
    {
        /* ------------------------------------- Menus -------------------------------------- */
        IEnumerable<Models.Menu> MenuEntitiesToModels(IEnumerable<Entities.Menu> menus);

        Models.Menu MenuEntityToModel(Entities.Menu menu);

        Entities.Menu MenuModelToEntity(Models.Menu menu);

        void MapMenuModelToEntity(Models.Menu menuMod, Entities.Menu menuEnt);


        /* ------------------------------------- Menu Categories ----------------------------- */
        IEnumerable<Models.MenuCategory> MenuCategoryEntitiesToModels(
            IEnumerable<Entities.MenuCategory> menuCats);

        Models.MenuCategory MenuCategoryEntityToModel(Entities.MenuCategory menuCategory);

        Entities.MenuCategory MenuCategoryModelToEntity(Models.MenuCategory menuCategory);

        void MapMenuCategoryModelToEntity(Models.MenuCategory menuCatMod, 
            Entities.MenuCategory menuCatEnt);


        /* ------------------------------------- Menu Items ----------------------------- */
        IEnumerable<Models.MenuItem> MenuItemEntitiesToModels(
            IEnumerable<Entities.MenuItem> menuItems);

        Models.MenuItem MenuItemEntityToModel(Entities.MenuItem menuItem);

        Entities.MenuItem MenuItemModelToEntity(Models.MenuItem menuItem);

        void MapMenuItemModelToEntity(Models.MenuItem menuItemMod,
            Entities.MenuItem menuItemEnt);
    }
}
