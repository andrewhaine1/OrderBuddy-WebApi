using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Ord.WebApi.Mappers.Order;
using Ord.WebApi.Models.Shared;
using Ord.WebApi.Services.Data.Orders;
using Ord.WebApi.Services.Data.Restaurants;
using Ord.WebApi.Services.Data.User;
using Ord.WebApi.Services.Web.Firebase;

namespace Ord.WebApi.Controllers
{
    [Route("api/orderinfo")]
    [ApiController]
    public class OrderInfoController : ControllerBase
    {
        private readonly IOrderService _ordService;
        private readonly IOrderMapper _ordMapper;
        private readonly IFcmService _fcmService;
        private readonly IOrdUserService _ordUserService;
        private readonly IRestaurantService _restService;

        public OrderInfoController(IOrderService orderService,
            IOrderMapper orderMapper,
            IOrdUserService ordUserService,
            IRestaurantService restService,
            IFcmService fcmService)
        {
            _ordService = orderService;
            _ordMapper = orderMapper;
            _fcmService = fcmService;
            _ordUserService = ordUserService;
            _restService = restService;
        }

        [AllowAnonymous]    // Owner/Admin only
        [HttpGet("{orderId}/", Name = "GetOrderInfo")]
        public async Task<IActionResult> GetOrderInfo(int orderId)
        {
            var orderInfoEnts = await _ordService.GetOrderInfoAsync(orderId);

            var orderInfoMods = _ordMapper.OrderInfoEntityToModel(orderInfoEnts);

            return Ok(orderInfoMods);
        }

        [AllowAnonymous]    // Should not be anonymous as order info can only be added by authenticated user.
        [HttpPost(Name = "AddOrderInfo")]
        public async Task<IActionResult> AddOrderInfo([FromBody] Models.OrderInfo orderInfoMod)
        {
            if (orderInfoMod == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            // Map restaurant model to entity.
            var orderInfoEnt = _ordMapper.OrderInfoModelToEntity(orderInfoMod);

            _ordService.AddOrderInfo(orderInfoEnt);

            if (!await _ordService.SaveOrderInfoChangesAsync())
                throw new Exception($"Could not add order info for order Id '{orderInfoMod.OrderId}'.");

            // Map newly saved restaurant back to model.
            orderInfoMod = _ordMapper.OrderInfoEntityToModel(orderInfoEnt);

            return CreatedAtRoute("GetOrderInfo",
                new { orderId = orderInfoMod.Id },
                orderInfoMod);
        }

        [AllowAnonymous]    // Should not be anonymous as order info can only be updated by authenticated user/owner and admin.
        [HttpPut("{orderInfoId}", Name = "PutUpdateOrderInfo")]
        public async Task<IActionResult> UpdateOrderInfo(int orderInfoId,
            [FromBody] Models.OrderInfo orderInfoMod)
        {
            if (orderInfoMod == null)
                return BadRequest();

            var orderInfoEnt = await _ordService.GetOrderInfoAsync(orderInfoId);
            if (orderInfoEnt == null)
                return NotFound(new { Error = $"An with Id '{orderInfoId}' could not be found." });

            TryValidateModel(orderInfoMod);

            // Add validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            _ordMapper.MapOrderInfoModelToEntity(orderInfoMod, orderInfoEnt);

            _ordService.UpdateOrderInfo(orderInfoEnt);

            if (!await _ordService.SaveOrderInfoChangesAsync())
                throw new Exception($"Error updating order info with Id '{orderInfoEnt.Id}'.");

            return NoContent();
        }

        [AllowAnonymous]    // Should not be anonymous as order info can only be added by authenticated user/owner and admin.
        [HttpPatch("{orderInfoId}", Name = "PatchUpdateOrderInfo")]
        public async Task<IActionResult> UpdateOrderInfo(int orderInfoId,
           [FromBody] JsonPatchDocument<Models.OrderInfo> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var orderInfoEnt = await _ordService.GetOrderInfoAsync(orderInfoId);
            if (orderInfoEnt == null)
                return NotFound(new { Error = $"Order info with the ID '{orderInfoId}' could not be found." });

            var orderInfoMod = _ordMapper.OrderInfoEntityToModel(orderInfoEnt);

            patchDoc.ApplyTo(orderInfoMod);

            TryValidateModel(orderInfoMod);

            // Validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            _ordMapper.MapOrderInfoModelToEntity(orderInfoMod, orderInfoEnt);

            _ordService.UpdateOrderInfo(orderInfoEnt);

            if (!await _ordService.SaveOrderChangesAsync())
                throw new Exception($"Error updating order info with Id '{orderInfoEnt.Id}'.");

            return NoContent();
        }

        [AllowAnonymous]    // Should not be anonymous as order info can only be added by authenticated user/owner and admin.
        [HttpDelete("{orderInfoId}", Name = "DeleteOrderInfo")]
        public async Task<IActionResult> DeleteOrderInfo(int orderInfoId)
        {
            var orderInfoEnt = await _ordService.GetOrderInfoAsync(orderInfoId);
            if (orderInfoEnt == null)
                return NotFound(new { Error = $"A order with the ID '{orderInfoId}' could not be found." });

            _ordService.DeleteOrderInfo(orderInfoEnt);

            if (!await _ordService.SaveOrderInfoChangesAsync())
                throw new Exception($"Error deleting order '{orderInfoEnt.Id}'.");

            return NoContent();
        }


        /* ======================================= Accept, Ready, Complete and Cancel orders ============================== */

        // All actions below are restricted to setting just the OrderStatus status state it is responsible for.
        // E.g. AcceptOrder sets stated to accepted, ReadyOrder sets status to ready etc.

        [AllowAnonymous]    // Should not be anonymous as order info can only be added by authenticated user/owner and admin.
        [HttpPatch("{orderInfoId}/acceptorder", Name = "AcceptOrder")]
        public async Task<IActionResult> AcceptOrder(int orderInfoId,
            [FromBody] JsonPatchDocument<Models.OrderInfo> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var orderInfoEnt = await _ordService.GetOrderInfoAsync(orderInfoId);
            if (orderInfoEnt == null)
                return NotFound(new { Error = $"Order info with the ID '{orderInfoId}' could not be found." });

            // Check that a valid OrderStatus Value was supplied.
            if (patchDoc.Operations.Count != 1 ||
                !Enum.TryParse(typeof(OrderStatus),
                (string)patchDoc.Operations[0].value,
                out object newOrderStatus))
                return BadRequest(new { Error = "Invalid order status name." });

            var orderStatus = (OrderStatus)newOrderStatus;
            var orderStatusName = Enum.GetName(typeof(OrderStatus), orderStatus);

            // Make sure incoming status is accepted only. Each status has its own action.
            // Also make sure that the existing order status is set to placed and not already 
            // accepted/ready/completed/cancelled.
            if (orderStatusName != Entities.Shared.OrderStatus.Accepted.ToString() ||
                orderInfoEnt.OrderStatus != Entities.Shared.OrderStatus.Placed)
                return BadRequest(new { Error = $"Invalid order status." });

            // Set accepted time to the current time
            var orderInfoMod = _ordMapper.OrderInfoEntityToModel(orderInfoEnt);
            orderInfoMod.DateTimeAccepted = DateTime.Now;

            // Apply patchdoc to model
            patchDoc.ApplyTo(orderInfoMod);

            TryValidateModel(orderInfoMod);

            // Validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            // Map model back to entity
            _ordMapper.MapOrderInfoModelToEntity(orderInfoMod, orderInfoEnt);

            _ordService.UpdateOrderInfo(orderInfoEnt);

            if (!await _ordService.SaveOrderChangesAsync())
                throw new Exception($"Error updating order info with Id '{orderInfoEnt.Id}'.");

            // Notift the user that the order has been accepted using Google Cloud Messaging
            var order = await _ordService.GetOrderAsync(orderInfoEnt.OrderId);
            var restaurant = await _restService.GetRestaurantAsync(order.RestaurantId);
            var user = await _ordUserService.GetOrdUserAsync(order.OrdUserId);

            if (order != null)
            {
                if (restaurant != null && user != null && user.DeviceToken != null)
                {
                    try
                    {
                        // *** Send message here *** //
                        await _fcmService.NotifyOrderAcceptedAsync(user.DeviceToken, restaurant.Name, order.Id);
                    }
                    catch(Exception) { }
                }
            }
            
            return NoContent();
        }

        [AllowAnonymous]    // Should not be anonymous as order info can only be added by authenticated user/owner and admin.
        [HttpPatch("{orderInfoId}/readyorder", Name = "ReadyOrder")]
        public async Task<IActionResult> ReadyOrder(int orderInfoId,
            [FromBody] JsonPatchDocument<Models.OrderInfo> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var orderInfoEnt = await _ordService.GetOrderInfoAsync(orderInfoId);
            if (orderInfoEnt == null)
                return NotFound(new { Error = $"Order info with the ID '{orderInfoId}' could not be found." });

            // Check that a valid OrderStatus Value was supplied.
            if (patchDoc.Operations.Count != 1 ||
                !Enum.TryParse(typeof(OrderStatus),
                (string)patchDoc.Operations[0].value,
                out object newOrderStatus))
                return BadRequest(new { Error = "Invalid order status name." });

            var orderStatus = (OrderStatus)newOrderStatus;
            var orderStatusName = Enum.GetName(typeof(OrderStatus), orderStatus);

            // Make sure incoming status is ready only. Each status has its own controller.
            // Also make sure that the existing order status is set to accepted and not already 
            // ready/completed.
            if (orderStatusName != Entities.Shared.OrderStatus.Ready.ToString() ||
                orderInfoEnt.OrderStatus != Entities.Shared.OrderStatus.Accepted)
                return BadRequest(new { Error = $"Invalid order status." });

            // Set accepted time to the current time
            var orderInfoMod = _ordMapper.OrderInfoEntityToModel(orderInfoEnt);
            orderInfoMod.DateTimeReady = DateTime.Now;

            // Apply patchdoc to model
            patchDoc.ApplyTo(orderInfoMod);

            TryValidateModel(orderInfoMod);

            // Validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            // Map model back to entity
            _ordMapper.MapOrderInfoModelToEntity(orderInfoMod, orderInfoEnt);

            _ordService.UpdateOrderInfo(orderInfoEnt);

            if (!await _ordService.SaveOrderChangesAsync())
                throw new Exception($"Error updating order info with Id '{orderInfoEnt.Id}'.");

            // Notift the user that the order has been accepted using Google Cloud Messaging
            var order = await _ordService.GetOrderAsync(orderInfoEnt.OrderId);
            var restaurant = await _restService.GetRestaurantAsync(order.RestaurantId);
            var user = await _ordUserService.GetOrdUserAsync(order.OrdUserId);

            if (order != null)
            {
                if (restaurant != null && user != null && user.DeviceToken != null)
                {
                    try
                    {
                        // *** Send message here *** //
                        await _fcmService.NotifyOrderReadyAsync(user.DeviceToken, restaurant.Name, order.Id);
                    }
                    catch (Exception) { }
                }
            }

            return NoContent();
        }

        [AllowAnonymous]    // Should not be anonymous as order info can only be added by authenticated user/owner and admin.
        [HttpPatch("{orderInfoId}/completeorder", Name = "CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderInfoId,
            [FromBody] JsonPatchDocument<Models.OrderInfo> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var orderInfoEnt = await _ordService.GetOrderInfoAsync(orderInfoId);
            if (orderInfoEnt == null)
                return NotFound(new { Error = $"Order info with the ID '{orderInfoId}' could not be found." });

            // Order should first be paid before it can be set to completed.
            if (!orderInfoEnt.IsPaid)
                return BadRequest(new { Error = $"Order has not been paid yet." });

            // Check that a valid OrderStatus Value was supplied.
            if (patchDoc.Operations.Count != 1 ||
                !Enum.TryParse(typeof(OrderStatus),
                (string)patchDoc.Operations[0].value,
                out object newOrderStatus))
                return BadRequest(new { Error = "Invalid order status name." });

            var orderStatus = (OrderStatus)newOrderStatus;
            var orderStatusName = Enum.GetName(typeof(OrderStatus), orderStatus);

            // Make sure incoming status is completed only. Each status has its own controller.
            // Also make sure that the existing order status is set to ready and not already 
            // completed.
            if (orderStatusName != Entities.Shared.OrderStatus.Completed.ToString() ||
                orderInfoEnt.OrderStatus != Entities.Shared.OrderStatus.Ready)
                return BadRequest(new { Error = $"Invalid order status." });

            // Set accepted time to the current time
            var orderInfoMod = _ordMapper.OrderInfoEntityToModel(orderInfoEnt);
            orderInfoMod.DateTimeCompleted = DateTime.Now;

            // Apply patchdoc to model
            patchDoc.ApplyTo(orderInfoMod);

            TryValidateModel(orderInfoMod);

            // Validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            // Map model back to entity
            _ordMapper.MapOrderInfoModelToEntity(orderInfoMod, orderInfoEnt);

            _ordService.UpdateOrderInfo(orderInfoEnt);

            if (!await _ordService.SaveOrderChangesAsync())
                throw new Exception($"Error updating order info with Id '{orderInfoEnt.Id}'.");

            // Notift the user that the order has been accepted using Google Cloud Messaging
            var order = await _ordService.GetOrderAsync(orderInfoEnt.OrderId);
            var restaurant = await _restService.GetRestaurantAsync(order.RestaurantId);
            var user = await _ordUserService.GetOrdUserAsync(order.OrdUserId);

            if (order != null)
            {
                if (restaurant != null && user != null && user.DeviceToken != null)
                {
                    try
                    {
                        // *** Send message here *** //
                        await _fcmService.NotifyOrderCompletedAsync(user.DeviceToken, restaurant.Name, order.Id);
                    }
                    catch (Exception) { }
                }
            }

            return NoContent();
        }

        [AllowAnonymous]    // Should not be anonymous as order info can only be added by authenticated user/owner and admin.
        [HttpPatch("{orderInfoId}/cancelorder", Name = "CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderInfoId,
            [FromBody] JsonPatchDocument<Models.OrderInfo> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var orderInfoEnt = await _ordService.GetOrderInfoAsync(orderInfoId);
            if (orderInfoEnt == null)
                return NotFound(new { Error = $"Order info with the ID '{orderInfoId}' could not be found." });

            // This could become an attribute for all order related controller actions.
            if (orderInfoEnt.IsCancelled)
                return BadRequest(new { Error = "Order has been cancelled." });

            // Check that a valid OrderStatus Value was supplied.
            if (patchDoc.Operations.Count != 1 ||
                !Enum.TryParse(typeof(OrderStatus),
                (string)patchDoc.Operations[0].value,
                out object newOrderStatus))
                return BadRequest(new { Error = "Invalid order status name." });

            var orderStatus = (OrderStatus)newOrderStatus;
            // var orderStatusName = Enum.GetName(typeof(OrderStatus), orderStatus);

            // Also make sure that the existing order status is set to placed or accepted and not already 
            // ready/completed/cancelled.
            if (orderInfoEnt.OrderStatus == Entities.Shared.OrderStatus.Ready ||
                orderInfoEnt.OrderStatus == Entities.Shared.OrderStatus.Completed ||
                orderInfoEnt.OrderStatus == Entities.Shared.OrderStatus.Cancelled)
                return BadRequest(new { Error = $"Invalid order status." });

            // Set cancelled time to the current time. ***Not yet implemented.
            var orderInfoMod = _ordMapper.OrderInfoEntityToModel(orderInfoEnt);
            orderInfoMod.DateTimeCancelled = DateTime.Now;

            orderInfoMod.IsCancelled = true;

            // Apply patchdoc to model
            patchDoc.ApplyTo(orderInfoMod);

            TryValidateModel(orderInfoMod);

            // Validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            // Map model back to entity
            _ordMapper.MapOrderInfoModelToEntity(orderInfoMod, orderInfoEnt);

            _ordService.UpdateOrderInfo(orderInfoEnt);

            if (!await _ordService.SaveOrderChangesAsync())
                throw new Exception($"Error updating order info with Id '{orderInfoEnt.Id}'.");

            // Notift the user that the order has been accepted using Google Cloud Messaging
            var order = await _ordService.GetOrderAsync(orderInfoEnt.OrderId);
            var restaurant = await _restService.GetRestaurantAsync(order.RestaurantId);
            var user = await _ordUserService.GetOrdUserAsync(order.OrdUserId);

            if (order != null)
            {
                if (restaurant != null && user != null && user.DeviceToken != null)
                {
                    try
                    {
                        // *** Send message here *** //
                        await _fcmService.NotifyOrderCancelledAsync(user.DeviceToken, restaurant.Name, order.Id);
                    }
                    catch (Exception) { }
                }
            }

            return NoContent();
        }

        [AllowAnonymous]    // Should not be anonymous as order info can only be added by authenticated user/owner and admin.
        [HttpPatch("{orderInfoId}/payorder", Name = "PayOrder")]
        public async Task<IActionResult> PayOrder(int orderInfoId,
            [FromBody] JsonPatchDocument<Models.OrderInfo> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var orderInfoEnt = await _ordService.GetOrderInfoAsync(orderInfoId);
            if (orderInfoEnt == null)
                return NotFound(new { Error = $"Order info with the ID '{orderInfoId}' could not be found." });

            // Check that only IsPaid boolean value was supplied.
            if (patchDoc.Operations.Count != 1 ||
                patchDoc.Operations[0].path != "/ispaid")
                return BadRequest(new { Error = "Only use to set orders to paid." });

            // Set accepted time to the current time
            var orderInfoMod = _ordMapper.OrderInfoEntityToModel(orderInfoEnt);
            orderInfoMod.IsPaid = true;

            // Apply patchdoc to model
            patchDoc.ApplyTo(orderInfoMod);

            TryValidateModel(orderInfoMod);

            // Validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            // Map model back to entity
            _ordMapper.MapOrderInfoModelToEntity(orderInfoMod, orderInfoEnt);

            _ordService.UpdateOrderInfo(orderInfoEnt);

            if (!await _ordService.SaveOrderChangesAsync())
                throw new Exception($"Error updating order info with Id '{orderInfoEnt.Id}'.");

            // Notift the user that the order has been accepted using Google Cloud Messaging

            // *** Send message here *** //

            return NoContent();
        }

        [AllowAnonymous]    // Should not be anonymous Owner only!.
        [HttpPatch("{orderInfoId}/paycompleteorder", Name = "PayCompleteOrder")]
        public async Task<IActionResult> PayCompleteOrder(int orderInfoId,
            [FromBody] JsonPatchDocument<Models.OrderInfo> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var orderInfoEnt = await _ordService.GetOrderInfoAsync(orderInfoId);
            if (orderInfoEnt == null)
                return NotFound(new { Error = $"Order info with the ID '{orderInfoId}' could not be found." });

            // Check that a valid OrderStatus Value was supplied.
            if (patchDoc.Operations.Count != 1 ||
                !Enum.TryParse(typeof(OrderStatus),
                (string)patchDoc.Operations[0].value,
                out object newOrderStatus))
                return BadRequest(new { Error = "Invalid order status name." });

            var orderStatus = (OrderStatus)newOrderStatus;
            var orderStatusName = Enum.GetName(typeof(OrderStatus), orderStatus);

            // Make sure incoming status is completed only. Each status has its own controller.
            // Also make sure that the existing order status is set to ready and not already 
            // completed.
            if (orderStatusName != Entities.Shared.OrderStatus.Completed.ToString() ||
                orderInfoEnt.OrderStatus != Entities.Shared.OrderStatus.Ready)
                return BadRequest(new { Error = $"Invalid order status." });

            // Set accepted time to the current time
            var orderInfoMod = _ordMapper.OrderInfoEntityToModel(orderInfoEnt);
            orderInfoMod.DateTimeCompleted = DateTime.Now;

            // Set order to paid or is paid to true as the customer most likely paid in the store.
            orderInfoMod.IsPaid = true;

            // Apply patchdoc to model
            patchDoc.ApplyTo(orderInfoMod);

            TryValidateModel(orderInfoMod);

            // Validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            // Map model back to entity
            _ordMapper.MapOrderInfoModelToEntity(orderInfoMod, orderInfoEnt);

            _ordService.UpdateOrderInfo(orderInfoEnt);

            if (!await _ordService.SaveOrderChangesAsync())
                throw new Exception($"Error updating order info with Id '{orderInfoEnt.Id}'.");

            // Notift the user that the order has been accepted using Google Cloud Messaging

            // *** Send message here *** //

            return NoContent();
        }
    }
}