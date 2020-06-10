using Microsoft.EntityFrameworkCore;
using Ord.WebApi.Entities;
using Ord.WebApi.Helpers.Paging;
using Ord.WebApi.Services.Data.Shared;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Ord.WebApi.Services.Data.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IDataRepository<Order> _orderRepo;
        private readonly IDataRepository<OrderInfo> _orderInfoRepo;
        private readonly IDataRepository<OrderItem> _orderItemRepo;

        public OrderService(IDataRepository<Order> orderRepo,
            IDataRepository<OrderInfo> orderInfoRepo,
            IDataRepository<OrderItem> orderItemRepo) {
            _orderRepo = orderRepo;
            _orderInfoRepo = orderInfoRepo;
            _orderItemRepo = orderItemRepo;
        }

        public async Task<PagedList<Order>> GetOrdersForRestaurantAsync(int restaurantId,
            ResourceParameters resourceParameters, DateRange dateRange)
        {

            if (dateRange == DateRange.Today)
            {
                var collection = _orderRepo.EntityDbSet
                .Where(o => o.OrderInfo.DateTimePlaced.Date == System.DateTime.Today)
                .Where(o => o.RestaurantId == restaurantId)
                .Include(o => o.Restaurant)
                .Include(o => o.OrdUser)
                .Include(o => o.OrderInfo)
                .Include(o => o.OrderItems)
                .ThenInclude(o => o.Item)
                .OrderByDescending(o => o.OrderInfo.DateTimePlaced);

                return await PagedList<Order>.CreateAsync(collection,
                resourceParameters.PageNumber,
                resourceParameters.PageSize);
            }

            if (dateRange == DateRange.LastSevenDays)
            {
                var collection = _orderRepo.EntityDbSet
                .Where(o => o.OrderInfo.DateTimePlaced >= System.DateTime.Now.AddDays(-7))
                .Where(o => o.RestaurantId == restaurantId)
                .Include(o => o.Restaurant)
                .Include(o => o.OrdUser)
                .Include(o => o.OrderInfo)
                .Include(o => o.OrderItems)
                .ThenInclude(o => o.Item)
                .OrderByDescending(o => o.OrderInfo.DateTimePlaced);
                

                return await PagedList<Order>.CreateAsync(collection,
                    resourceParameters.PageNumber,
                    resourceParameters.PageSize);
            }

            return await PagedList<Order>.CreateAsync(_orderRepo.EntityDbSet
                .Where(o => o.RestaurantId == restaurantId)
                .Include(o => o.Restaurant)
                .Include(o => o.OrdUser)
                .Include(o => o.OrderInfo)
                .Include(o => o.OrderItems)
                .ThenInclude(o => o.Item)
                .OrderByDescending(o => o.OrderInfo.DateTimePlaced),
            resourceParameters.PageNumber,
            resourceParameters.PageSize);
        }      

        public async Task<PagedList<Order>> GetOrdersForUserAsync(int userId,
            ResourceParameters resourceParameters, DateRange dateRange)
        {

            if (dateRange == DateRange.Today)
            {
                var collection = _orderRepo.EntityDbSet
                .Where(o => o.OrderInfo.DateTimePlaced.Date == System.DateTime.Today)
                .Where(o => o.OrdUserId == userId)
                .Include(o => o.Restaurant)
                .Include(o => o.OrdUser)
                .Include(o => o.OrderInfo)
                .Include(o => o.OrderItems)
                .ThenInclude(o => o.Item)
                .OrderByDescending(o => o.OrderInfo.DateTimePlaced);

                return await PagedList<Order>.CreateAsync(collection,
                resourceParameters.PageNumber,
                resourceParameters.PageSize);
            }

            if (dateRange == DateRange.LastSevenDays)
            {
                var collection = _orderRepo.EntityDbSet
                .Where(o => o.OrderInfo.DateTimePlaced >= System.DateTime.Now.AddDays(-7))
                .Where(o => o.OrdUserId == userId)
                .Include(o => o.Restaurant)
                .Include(o => o.OrdUser)
                .Include(o => o.OrderInfo)
                .Include(o => o.OrderItems)
                .ThenInclude(o => o.Item)
                .OrderByDescending(o => o.OrderInfo.DateTimePlaced);


                return await PagedList<Order>.CreateAsync(collection,
                    resourceParameters.PageNumber,
                    resourceParameters.PageSize);
            }

            return await PagedList<Order>.CreateAsync(_orderRepo.EntityDbSet
                .Where(o => o.OrdUserId == userId)
                .Include(o => o.Restaurant)
                .Include(o => o.OrdUser)
                .Include(o => o.OrderInfo)
                .Include(o => o.OrderItems)
                .ThenInclude(o => o.Item)
                .OrderByDescending(o => o.OrderInfo.DateTimePlaced),
            resourceParameters.PageNumber,
            resourceParameters.PageSize);
        }

        public async Task<Order> GetOrderAsync(int orderId)
            => await _orderRepo.EntityDbSet
                .Where(o => o.Id == orderId)
                .Include(o => o.Restaurant)
                .Include(o => o.OrdUser)
                .Include(o => o.OrderInfo)
                .Include(o => o.OrderItems)
                .ThenInclude(o => o.Item)
                .FirstOrDefaultAsync();

        public void AddOrder(Order order)
            => _orderRepo.AddEntity(order);

        public void UpdateOrder(Order order)
            => _orderRepo.UpdateEntity(order);

        public void DeleteOrder(Order order)
            => _orderRepo.DeleteEntity(order);

        public async Task<bool> SaveOrderChangesAsync()
            => await _orderRepo.SaveChangesAsync();


        /* ------------------------------ Order Info ----------------------------- */

        public async Task<OrderInfo> GetOrderInfoAsync(int orderId)
            => await _orderInfoRepo.GetEntityAsync(orderId);

        public void AddOrderInfo(OrderInfo orderInfo)
            => _orderInfoRepo.AddEntity(orderInfo);

        public void UpdateOrderInfo(OrderInfo orderInfo)
            => _orderInfoRepo.UpdateEntity(orderInfo);

        public void DeleteOrderInfo(OrderInfo orderInfo)
            => _orderInfoRepo.DeleteEntity(orderInfo);

        public async Task<bool> SaveOrderInfoChangesAsync()
            => await _orderInfoRepo.SaveChangesAsync();

        /* ------------------------------ Order Items ----------------------------- */

        public async Task<PagedList<OrderItem>> GetOrderItemsAsync(int orderId,
            ResourceParameters resourceParameters)
            => await PagedList<OrderItem>.CreateAsync(
                _orderItemRepo.EntityDbSet
                .Where(o => o.OrderId == orderId)
                .Include(o => o.Order)
                .Include(o => o.Item),
                resourceParameters.PageNumber,
                resourceParameters.PageSize);

        public async Task<OrderItem> GetOrderItemAsync(int orderItemId)
            => await _orderItemRepo.GetEntityAsync(orderItemId);

        public void AddOrderItem(OrderItem orderItem)
            => _orderItemRepo.AddEntity(orderItem);

        public void UpdateOrderItem(OrderItem orderItem)
            => _orderItemRepo.UpdateEntity(orderItem);

        public void DeleteOrderItem(OrderItem orderItem)
            => _orderItemRepo.DeleteEntity(orderItem);

        public async Task<bool> SaveOrderItemChangesAsync()
            => await _orderItemRepo.SaveChangesAsync();
    }
}
