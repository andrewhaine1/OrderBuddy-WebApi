using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ord.WebApi.Services.Web.Firebase
{
    public interface IFcmService
    {
        Task<bool> NotifyOrderPlacedAsync(string to, string fullName, int orderId);
        Task<bool> NotifyOrderAcceptedAsync(string to, string restaurantName, int orderId);
        Task<bool> NotifyOrderRejectedAsync(string to, string restaurantName, int orderId);
        Task<bool> NotifyOrderReadyAsync(string to, string restaurantName, int orderId);
        Task<bool> NotifyOrderCancelledAsync(string to, string restaurantName, int orderId);
        Task<bool> NotifyOrderCompletedAsync(string to, string restaurantName, int orderId);
    }
}
