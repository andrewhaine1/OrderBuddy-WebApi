using Ord.WebApi.Helpers.Paging;
using System.Collections.Generic;

namespace Ord.WebApi.Mappers.Order
{
    public interface IOrderMapper
    {
        /* ------------------------------------- Orders -------------------------------------- */
        PagedList<Models.Order> OrderEntitiesToModels(PagedList<Entities.Order> orders);

        Models.Order OrderEntityToModel(Entities.Order order);

        Entities.Order OrderModelToEntity(Models.Order order);

        void MapOrderModelToEntity(Models.Order orderMod, Entities.Order orderEnt);


        /* ------------------------------------- OrderInfo -------------------------------------- */
        IEnumerable<Models.OrderInfo> OrderInfoEntitiesToModels(IEnumerable<Entities.OrderInfo> orderInfos);

        Models.OrderInfo OrderInfoEntityToModel(Entities.OrderInfo orderInfo);

        Entities.OrderInfo OrderInfoModelToEntity(Models.OrderInfo orderInfo);

        void MapOrderInfoModelToEntity(Models.OrderInfo orderInfoMod, Entities.OrderInfo orderInfoEnt);


        /* ------------------------------------- OrderItems -------------------------------------- */
        IEnumerable<Models.OrderItem> OrderItemEntitiesToModels(IEnumerable<Entities.OrderItem> orderItems);

        IEnumerable<Entities.OrderItem> OrderItemModelsToEntites(IEnumerable<Models.OrderItem> OrderItems);

        Models.OrderItem OrderItemEntityToModel(Entities.OrderItem orderItem);

        Entities.OrderItem OrderItemModelToEntity(Models.OrderItem orderItem);

        void MapOrderItemModelToEntity(Models.OrderItem orderItemMod, Entities.OrderItem orderItemEnt);
    }
}
