using AutoMapper;
using Ord.WebApi.Helpers.Paging;
using System.Collections.Generic;

namespace Ord.WebApi.Mappers.Order
{
    public class OrderMapper : IOrderMapper
    {
	/* ------------------------------------- Ord Users ------------------------------------ */
	public Models.OrdUserForOrder OrdUserEntityOrdUserForOrderToModel(Entities.OrdUser ordUser)
           => new MapperConfiguration(cfg => cfg.CreateMap<Entities.OrdUser,
               Models.OrdUserForOrder>()
               .ForMember(o => o.FullName, opt => opt.MapFrom(src => src.FullName))
               .ForMember(o => o.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
               .ForMember(o => o.MobileNumber, opt => opt.MapFrom(src => src.MobileNumber)))
               .CreateMapper()
               .Map<Models.OrdUserForOrder>(ordUser);

        /* ------------------------------------- Orders -------------------------------------- */
        public PagedList<Models.Order> OrderEntitiesToModels(PagedList<Entities.Order> orders)
            => new MapperConfiguration(cfg => cfg.CreateMap<Entities.Order,
                Models.Order>()
                .ForMember(o => o.OrderInfo, opt => opt.MapFrom(src => 
                OrderInfoEntityToModel(src.OrderInfo)))
                .ForMember(o => o.OrderItems, opt => opt.MapFrom(src =>
                OrderItemEntitiesToModels(src.OrderItems)))
		.ForMember(o => o.OrdUser, opt => opt.MapFrom(src => OrdUserEntityOrdUserForOrderToModel(src.OrdUser))))
                .CreateMapper()
                .Map<PagedList<Models.Order>>(orders);

        public Models.Order OrderEntityToModel(Entities.Order order)
            => new MapperConfiguration(cfg => cfg.CreateMap<Entities.Order,
                Models.Order>()
                .ForMember(o => o.OrderInfo, opt => opt.MapFrom(src =>
                OrderInfoEntityToModel(src.OrderInfo)))
                .ForMember(o => o.OrderItems, opt => opt.MapFrom(src =>
                OrderItemEntitiesToModels(src.OrderItems)))
		.ForMember(o => o.OrdUser, opt => opt.MapFrom(src => OrdUserEntityOrdUserForOrderToModel(src.OrdUser))))
                .CreateMapper()
                .Map<Models.Order>(order);

        public Entities.Order OrderModelToEntity(Models.Order order)
            => new MapperConfiguration(cfg => cfg.CreateMap<Models.Order,
                Entities.Order>()
                .ForMember(o => o.OrderInfo, opt => opt.MapFrom(src =>
                OrderInfoModelToEntity(src.OrderInfo)))
                .ForMember(o => o.OrderItems, opt => opt.MapFrom(src =>
                OrderItemModelsToEntites(src.OrderItems))))
                .CreateMapper()
                .Map<Entities.Order>(order);

        public void MapOrderModelToEntity(Models.Order orderMod, Entities.Order orderEnt)
        {
            var mapper = new MapperConfiguration(configure =>
                configure.CreateMap<Models.Order, Entities.Order>()
                .ForMember(o => o.OrderInfo, opt => opt.MapFrom(src =>
                OrderInfoModelToEntity(src.OrderInfo)))
                .ForMember(o => o.OrderItems, opt => opt.MapFrom(src =>
                OrderItemModelsToEntites(src.OrderItems))))
                .CreateMapper();

            mapper.Map(orderMod, orderEnt);
        }


        /* ------------------------------------- OrderInfo -------------------------------------- */
        public IEnumerable<Models.OrderInfo> OrderInfoEntitiesToModels(IEnumerable<Entities.OrderInfo> orderInfos)
            => new MapperConfiguration(cfg => cfg.CreateMap<Entities.OrderInfo,
                Models.OrderInfo>())
                .CreateMapper()
                .Map<IEnumerable<Models.OrderInfo>>(orderInfos);


        public Models.OrderInfo OrderInfoEntityToModel(Entities.OrderInfo orderInfo)
            => new MapperConfiguration(cfg => cfg.CreateMap<Entities.OrderInfo,
                Models.OrderInfo>())
                .CreateMapper()
                .Map<Models.OrderInfo>(orderInfo);

        public Entities.OrderInfo OrderInfoModelToEntity(Models.OrderInfo orderInfo)
            => new MapperConfiguration(cfg => cfg.CreateMap<Models.OrderInfo,
                Entities.OrderInfo>())
                .CreateMapper()
                .Map<Entities.OrderInfo>(orderInfo);

        public void MapOrderInfoModelToEntity(Models.OrderInfo orderInfoMod, Entities.OrderInfo orderInfoEnt)
        {
            var mapper = new MapperConfiguration(configure =>
                configure.CreateMap<Models.OrderInfo, Entities.OrderInfo>())
                .CreateMapper();

            mapper.Map(orderInfoMod, orderInfoEnt);
        }


        /* ------------------------------------- OrderItems -------------------------------------- */
        public IEnumerable<Models.OrderItem> OrderItemEntitiesToModels(IEnumerable<Entities.OrderItem> orderItems)
            => new MapperConfiguration(cfg => cfg.CreateMap<Entities.OrderItem,
                Models.OrderItem>()
                .ForMember(o => o.Name, opt => opt.MapFrom(src => 
                src.Item.Name)))
                .CreateMapper()
                .Map<IEnumerable<Models.OrderItem>>(orderItems);

        public IEnumerable<Entities.OrderItem> OrderItemModelsToEntites(IEnumerable<Models.OrderItem> OrderItems)
            => new MapperConfiguration(cfg => cfg.CreateMap<Models.OrderItem,
                Entities.OrderItem>()
                .ForMember(o => o.Order, opt => opt.Ignore())
                .ForMember(o => o.Item, opt => opt.Ignore()))
                .CreateMapper()
                .Map<IEnumerable<Entities.OrderItem>>(OrderItems);

        public Models.OrderItem OrderItemEntityToModel(Entities.OrderItem orderItem)
            => new MapperConfiguration(cfg => cfg.CreateMap<Entities.OrderItem,
                Models.OrderItem>()
                .ForMember(o => o.Name, opt => opt.MapFrom(src =>
                src.Item.Name)))
                .CreateMapper()
                .Map<Models.OrderItem>(orderItem);

        public Entities.OrderItem OrderItemModelToEntity(Models.OrderItem orderItem)
            => new MapperConfiguration(cfg => cfg.CreateMap<Models.OrderItem,
                Entities.OrderItem>())
                .CreateMapper()
                .Map<Entities.OrderItem>(orderItem);

        public void MapOrderItemModelToEntity(Models.OrderItem orderItemMod, Entities.OrderItem orderItemEnt)
        {
            var mapper = new MapperConfiguration(configure =>
                configure.CreateMap<Models.OrderItem, Entities.OrderItem>())
                .CreateMapper();

            mapper.Map(orderItemMod, orderItemEnt);
        }
    }
}
