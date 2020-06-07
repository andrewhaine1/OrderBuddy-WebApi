using Ord.WebApi.Entities;
using Ord.WebApi.Helpers.Paging;
using System.Threading.Tasks;

namespace Ord.WebApi.Services.Data.Orders
{
    public interface IOrderService
    {
        /* ------------------------------ Orders ----------------------------- */

        // This method is for restaurant owners and admins.
        Task<PagedList<Order>> GetOrdersForRestaurantAsync(int restaurantId, 
            ResourceParameters resourceParameters, DateRange dateRange);

        Task<PagedList<Order>> GetOrdersForUserAsync(int userId,
            ResourceParameters resourceParameters, DateRange dateRange);

        Task<Order> GetOrderAsync(int orderId);

        void AddOrder(Order order);

        void UpdateOrder(Order order);

        void DeleteOrder(Order order);

        Task<bool> SaveOrderChangesAsync();

        /* ------------------------------ Order Info ----------------------------- */

        // This method is for restaurant owners and admins.
        Task<OrderInfo> GetOrderInfoAsync(int orderId);

        void AddOrderInfo(OrderInfo orderInfo);

        void UpdateOrderInfo(OrderInfo orderInfo);

        void DeleteOrderInfo(OrderInfo orderInfo);

        Task<bool> SaveOrderInfoChangesAsync();

        /* ------------------------------ Order Items ----------------------------- */

        // This method is for restaurant owners and admins.
        Task<PagedList<OrderItem>> GetOrderItemsAsync(int orderId, 
            ResourceParameters resourceParameters);

        Task<OrderItem> GetOrderItemAsync(int oderItemId);

        void AddOrderItem(OrderItem orderItem);

        void UpdateOrderItem(OrderItem orderItem);

        void DeleteOrderItem(OrderItem orderItem);

        Task<bool> SaveOrderItemChangesAsync();
    }
}
