using System.Collections.Generic;
using AutoMapper;

namespace Ord.WebApi.Mappers.Menu
{
    public class MenuMapper : IMenuMapper
    {
        /* ------------------------------------- Menus -------------------------------------- */
        public IEnumerable<Models.Menu> MenuEntitiesToModels(IEnumerable<Entities.Menu> menus)
            => new MapperConfiguration(cfg => cfg.CreateMap<Entities.Menu,
                Models.Menu>())
                .CreateMapper()
                .Map<IEnumerable<Models.Menu>>(menus);

        public Models.Menu MenuEntityToModel(Entities.Menu menu)
            => new MapperConfiguration(cfg => cfg.CreateMap<Entities.Menu,
                Models.Menu>())
                .CreateMapper()
                .Map<Models.Menu>(menu);

        public Entities.Menu MenuModelToEntity(Models.Menu menu)
            => new MapperConfiguration(cfg => cfg.CreateMap<Models.Menu,
                Entities.Menu>())
                .CreateMapper()
                .Map<Entities.Menu>(menu);

        public void MapMenuModelToEntity(Models.Menu menuMod, Entities.Menu menuEnt)
        {
            var mapper = new MapperConfiguration(configure =>
                configure.CreateMap<Models.Menu, Entities.Menu>())
                .CreateMapper();

            mapper.Map(menuMod, menuEnt);
        }

        /* ------------------------------------- Menu Categories ----------------------------- */
        public IEnumerable<Models.MenuCategory> MenuCategoryEntitiesToModels(
            IEnumerable<Entities.MenuCategory> menuCats)
            => new MapperConfiguration(cfg => cfg.CreateMap<Entities.MenuCategory,
                Models.MenuCategory>()
                .ForMember(dest => dest.MenuItems, opt => opt.MapFrom(src =>
                MenuItemEntitiesToModels(src.MenuItems))))
                .CreateMapper()
                .Map<IEnumerable<Models.MenuCategory>>(menuCats);

        public Models.MenuCategory MenuCategoryEntityToModel(Entities.MenuCategory menuCategory)
            => new MapperConfiguration(cfg => cfg.CreateMap<Entities.MenuCategory,
                Models.MenuCategory>())
                .CreateMapper()
                .Map<Models.MenuCategory>(menuCategory);

        public Entities.MenuCategory MenuCategoryModelToEntity(Models.MenuCategory menuCategory)
            => new MapperConfiguration(cfg => cfg.CreateMap<Models.MenuCategory, 
                Entities.MenuCategory>()
                .ForMember(dest => dest.Menu, opt => opt.Ignore()))
                .CreateMapper()
                .Map<Entities.MenuCategory>(menuCategory);

        public void MapMenuCategoryModelToEntity(Models.MenuCategory menuCatMod,
            Entities.MenuCategory menuCatEnt)
        {
            var mapper = new MapperConfiguration(configure =>
                configure.CreateMap<Models.MenuCategory, Entities.MenuCategory>()
                .ForMember(dest => dest.Menu, opt => opt.Ignore()))
                .CreateMapper();

            mapper.Map(menuCatMod, menuCatEnt);
        }


        /* ------------------------------------- Menu Items ----------------------------- */
        public IEnumerable<Models.MenuItem> MenuItemEntitiesToModels(
            IEnumerable<Entities.MenuItem> menuItems)
            => new MapperConfiguration(cfg => cfg.CreateMap<Entities.MenuItem,
                Models.MenuItem>())
                //.ForMember(dest => dest.Price, opt => opt.MapFrom(src => 
                //src.Price.ToString())))
                .CreateMapper()
                .Map<IEnumerable<Models.MenuItem>>(menuItems);

        public Models.MenuItem MenuItemEntityToModel(Entities.MenuItem menuItem)
            => new MapperConfiguration(cfg => cfg.CreateMap<Entities.MenuItem,
                Models.MenuItem>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src =>
                src.Price.ToString())))
                .CreateMapper()
                .Map<Models.MenuItem>(menuItem);

        public Entities.MenuItem MenuItemModelToEntity(Models.MenuItem menuItem)
            => new MapperConfiguration(cfg => cfg.CreateMap<Models.MenuItem, 
                Entities.MenuItem>()
                .ForMember(dest => dest.Category, opt => opt.Ignore()))
                //.ForMember(dest => dest.Price, opt => opt.MapFrom(src => 
                //decimal.Parse(src.Price))))
                .CreateMapper()
                .Map<Entities.MenuItem>(menuItem);

        public void MapMenuItemModelToEntity(Models.MenuItem menuItemMod,
            Entities.MenuItem menuItemEnt)
        {
            var mapper = new MapperConfiguration(configure =>
                configure.CreateMap<Models.MenuItem, Entities.MenuItem>()
                .ForMember(dest => dest.Category, opt => opt.Ignore()))
                //.ForMember(dest => dest.Price, opt => opt.MapFrom(src =>
                //decimal.Parse(src.Price))))
                .CreateMapper();

            mapper.Map(menuItemMod, menuItemEnt);
        }
    }
}
