using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Ord.WebApi.Helpers.Paging;
using Ord.WebApi.Mappers.Order;
using Ord.WebApi.Services.Data.Orders;

namespace Ord.WebApi.Controllers
{
    [Route("api/orderitems")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly IOrderService _ordService;
        private readonly IOrderMapper _ordMapper;

        public OrderItemsController(IOrderService orderService,
            IOrderMapper orderMapper)
        {
            _ordService = orderService;
            _ordMapper = orderMapper;
        }

        [AllowAnonymous]    // Owner/Admin only
        [HttpGet("{orderId}/", Name = "GetOrderItems")]
        public async Task<IActionResult> GetOrderItems(int orderId,
            OrdersResourceParameters orderParams)
        {
            var orderItemEnts = await _ordService.GetOrderItemsAsync(orderId,
                orderParams);

            var orderItemMods = _ordMapper.OrderItemEntitiesToModels(orderItemEnts);

            return Ok(orderItemMods);
        }

        [AllowAnonymous]    // Should not be anonymous as order info can only be added by authenticated user.
        [HttpPost(Name = "AddOrderItem")]
        public async Task<IActionResult> AddOrderItem([FromBody] Models.OrderItem orderItemMod)
        {
            if (orderItemMod == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            // Map restaurant model to entity.
            var orderInfoEnt = _ordMapper.OrderItemModelToEntity(orderItemMod);

            _ordService.AddOrderItem(orderInfoEnt);

            if (!await _ordService.SaveOrderItemChangesAsync())
                throw new Exception($"Could not add item to order with Id '{orderItemMod.OrderId}'.");

            // Map newly saved restaurant back to model.
            orderItemMod = _ordMapper.OrderItemEntityToModel(orderInfoEnt);

            return CreatedAtRoute("GetOrderItem",
                new { orderId = orderItemMod.Id },
                orderItemMod);
        }

        [AllowAnonymous]    // Should not be anonymous as order info can only be updated by authenticated user/owner and admin.
        [HttpPut("{orderItemId}", Name = "PutUpdateOrderItem")]
        public async Task<IActionResult> UpdateOrderItem(int orderItemId,
            [FromBody] Models.OrderItem orderItemMod)
        {
            if (orderItemMod == null)
                return BadRequest();

            var orderItemEnt = await _ordService.GetOrderItemAsync(orderItemId);
            if (orderItemEnt == null)
                return NotFound(new { Error = $"An with Id '{orderItemId}' could not be found." });

            TryValidateModel(orderItemMod);

            // Add validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            _ordMapper.MapOrderItemModelToEntity(orderItemMod, orderItemEnt);

            _ordService.UpdateOrderItem(orderItemEnt);

            if (!await _ordService.SaveOrderItemChangesAsync())
                throw new Exception($"Error updating order item with Id '{orderItemEnt.Id}'.");

            return NoContent();
        }

        [AllowAnonymous]    // Should not be anonymous as order info can only be added by authenticated user/owner and admin.
        [HttpPatch("{orderItemId}", Name = "PatchUpdateOrderItem")]
        public async Task<IActionResult> UpdateOrderItem(int orderItemId,
           [FromBody] JsonPatchDocument<Models.OrderItem> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var orderItemEnt = await _ordService.GetOrderItemAsync(orderItemId);
            if (orderItemEnt == null)
                return NotFound(new { Error = $"Order info with the ID '{orderItemId}' could not be found." });

            var orderItemMod = _ordMapper.OrderItemEntityToModel(orderItemEnt);

            patchDoc.ApplyTo(orderItemMod);

            TryValidateModel(orderItemMod);

            // Validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            _ordMapper.MapOrderItemModelToEntity(orderItemMod, orderItemEnt);

            _ordService.UpdateOrderItem(orderItemEnt);

            if (!await _ordService.SaveOrderItemChangesAsync())
                throw new Exception($"Error updating order item with Id '{orderItemEnt.Id}'.");

            return NoContent();
        }

        [AllowAnonymous]    // Should not be anonymous as order info can only be added by authenticated user/owner and admin.
        [HttpDelete("{orderItemId}", Name = "DeleteOrderItem")]
        public async Task<IActionResult> DeleteOrderItem(int orderItemId)
        {
            var orderItemEnt = await _ordService.GetOrderItemAsync(orderItemId);
            if (orderItemEnt == null)
                return NotFound(new { Error = $"A order with the ID '{orderItemId}' could not be found." });

            _ordService.DeleteOrderItem(orderItemEnt);

            if (!await _ordService.SaveOrderItemChangesAsync())
                throw new Exception($"Error deleting order item with Id '{orderItemEnt.Id}'.");

            return NoContent();
        }
    }
}