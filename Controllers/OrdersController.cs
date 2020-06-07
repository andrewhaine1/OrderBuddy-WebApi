using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Ord.WebApi.Helpers.Paging;
using Ord.WebApi.Mappers.Order;
using Ord.WebApi.Services.Data.Orders;
using Ord.WebApi.Services.Data.Restaurants;
using Ord.WebApi.Services.Data.User;
using Ord.WebApi.Services.Web.Firebase;

namespace Ord.WebApi.Controllers
{
    [Authorize]
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _ordService;
        private readonly IOrderMapper _ordMapper;
        private readonly IFcmService _fcmService;
        private readonly IOrdUserService _ordUserService;
        private readonly IRestaurantService _restService;

        public OrdersController(IOrderService orderService,
            IOrderMapper orderMapper,
            IFcmService fcmService,
            IOrdUserService ordUserService,
            IRestaurantService restService)
        {
            _ordService = orderService;
            _ordMapper = orderMapper;
            _fcmService = fcmService;
            _ordUserService = ordUserService;
            _restService = restService;
        }

        [AllowAnonymous]    // Owner/Admin only
        [HttpGet(Name = "GetOrders")]
        public async Task<IActionResult> GetOrders([FromQuery]
            OrdersResourceParameters orderParams)
        {
            // Check that a valid OrderStatus Value was supplied.
            if (!Enum.TryParse(typeof(DateRange), orderParams.DateRange, out object dateRange))
                return BadRequest(new { Error = "Invalid date range." });

            if (orderParams.RestId < 1 && orderParams.UserId < 1)
                return NotFound(new
                {
                    Error = "Please make sure you have specified a valid " +
                    "restaurant or user Id e.g. (restId = 10 or userId = 10)."
                });

            if (orderParams.RestId > 0)
            {
                var orderEnts = await _ordService.GetOrdersForRestaurantAsync(orderParams.RestId,
                    orderParams, (DateRange)dateRange);

                var orderMods = _ordMapper.OrderEntitiesToModels(orderEnts);

                var paginationMetadata = new
                {
                    totalCount = orderEnts.TotalCount,
                    pageSize = orderEnts.PageSize,
                    currentPage = orderEnts.CurrentPage,
                    totalPages = orderEnts.TotalPages
                };

                return Ok(new { Values = orderMods, PagingInfo = paginationMetadata });
            }

            if (orderParams.UserId > 0)
            {
                var orderEnts = await _ordService.GetOrdersForUserAsync(orderParams.UserId,
                    orderParams, (DateRange)dateRange);

                var orderMods = _ordMapper.OrderEntitiesToModels(orderEnts);

                var paginationMetadata = new
                {
                    totalCount = orderEnts.TotalCount,
                    pageSize = orderEnts.PageSize,
                    currentPage = orderEnts.CurrentPage,
                    totalPages = orderEnts.TotalPages
                };

                return Ok(new { Values = orderMods, PagingInfo = paginationMetadata });
            }

            return NotFound(new { Warning = "You most likely did not specify a restaurantId or userId query parameter." });
        }

        [AllowAnonymous]    // Owner/Admin/Authenticated user
        [HttpGet("{orderId}", Name = "GetOrder")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var order = await _ordService.GetOrderAsync(orderId);

            if (order == null)
                return BadRequest(new
                {
                    Error = $"An order with the Id '{orderId}' " +
                    $"could not be found."
                });

            var model = _ordMapper.OrderEntityToModel(order);

            return Ok(model);
        }

        // Should not be anonymous as order can only be added by authenticated user.
        [HttpPost(Name = "AddOrder")]
        public async Task<IActionResult> AddOrder([FromBody] Models.Order orderMod)
        {
            if (orderMod == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            // Map restaurant model to entity.
            var orderEnt = _ordMapper.OrderModelToEntity(orderMod);

            _ordService.AddOrder(orderEnt);

            if (!await _ordService.SaveOrderChangesAsync())
                throw new Exception($"Could not place order for user with Id '{orderMod.OrdUserId}'.");

            // Notift the user that the order has been accepted using Google Cloud Messaging
            var restaurant = await _restService.GetRestaurantAsync(orderEnt.RestaurantId);
            var owner = await _ordUserService.GetOrdUserAsync(restaurant.OrdUserId);
            var user = await _ordUserService.GetOrdUserAsync(orderEnt.OrdUserId);

            if (restaurant != null)
            {
                if (owner != null && owner.DeviceToken != null && user != null)
                {
                    try
                    {
                        // *** Send message here *** //
                        await _fcmService.NotifyOrderPlacedAsync(owner.DeviceToken, user.FullName, orderEnt.Id);
                    }
                    catch (Exception) { }
                }
            }

            // Map newly saved restaurant back to model.
            orderMod = _ordMapper.OrderEntityToModel(orderEnt);

            return CreatedAtRoute("GetOrder",
                new { orderId = orderMod.Id },
                orderMod);
        }

        [AllowAnonymous]    // Should not be anonymous as order can only be updated by authenticated user/owner and admin.
        [HttpPut("{orderId}", Name = "PutUpdateOrder")]
        public async Task<IActionResult> UpdateOrder(int orderId,
            [FromBody] Models.Order orderMod)
        {
            if (orderMod == null)
                return BadRequest();

            var orderEnt = await _ordService.GetOrderAsync(orderId);
            if (orderEnt == null)
                return NotFound(new { Error = $"An with Id '{orderId}' could not be found." });

            TryValidateModel(orderMod);

            // Add validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            _ordMapper.MapOrderModelToEntity(orderMod, orderEnt);

            _ordService.UpdateOrder(orderEnt);

            if (!await _ordService.SaveOrderChangesAsync())
                throw new Exception($"Error updating order '{orderEnt.Id}'.");

            return NoContent();
        }

        [AllowAnonymous]    // Should not be anonymous as restaurant can only be added by authenticated user/owner and admin.
        [HttpPatch("{orderId}", Name = "PatchUpdateOrder")]
        public async Task<IActionResult> UpdateOrder(int orderId,
           [FromBody] JsonPatchDocument<Models.Order> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var orderEnt = await _ordService.GetOrderAsync(orderId);
            if (orderEnt == null)
                return NotFound(new { Error = $"A restaurant with the ID '{orderId}' could not be found." });

            var orderMod = _ordMapper.OrderEntityToModel(orderEnt);

            patchDoc.ApplyTo(orderMod);

            TryValidateModel(orderMod);

            // Validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            _ordMapper.MapOrderModelToEntity(orderMod, orderEnt);

            _ordService.UpdateOrder(orderEnt);

            if (!await _ordService.SaveOrderChangesAsync())
                throw new Exception($"Error updating order with Id '{orderEnt.Id}'.");

            return NoContent();
        }

        [AllowAnonymous]    // Should not be anonymous as restaurant can only be added by authenticated user/owner and admin.
        [HttpDelete("{orderId}", Name = "DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var orderEnt = await _ordService.GetOrderAsync(orderId);
            if (orderEnt == null)
                return NotFound(new { Error = $"A order with the ID '{orderId}' could not be found." });

            _ordService.DeleteOrder(orderEnt);

            if (!await _ordService.SaveOrderChangesAsync())
                throw new Exception($"Error deleting order '{orderEnt.Id}'.");

            return NoContent();
        }
    }
}