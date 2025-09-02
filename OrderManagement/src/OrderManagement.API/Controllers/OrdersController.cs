using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Features.Orders.Commands.ConfirmOrder;
using OrderManagement.Application.Features.Orders.Commands.CreateOrder;
using OrderManagement.Application.Features.Orders.Commands.UpdateOrderStatus;
using OrderManagement.Application.Features.Orders.Queries.GetOrder;
using OrderManagement.Application.Features.Orders.Queries.GetOrders;

namespace OrderManagement.API.Controllers
{
    /// <summary>
    /// Order management endpoints
    /// </summary>
    [Route("api/[controller]")]
    public class OrdersController : BaseApiController
    {
        /// <summary>
        /// Get all orders with optional filtering and pagination
        /// </summary>
        /// <param name="query">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of orders</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetOrders([FromQuery] GetOrdersQuery query)
        {
            var result = await Mediator.Send(query);
            return HandlePaginatedResult(result);
        }

        /// <summary>
        /// Get a specific order by ID
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Order details</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetOrder(Guid id)
        {
            var result = await Mediator.Send(new GetOrderQuery(id));
            return HandleResult(result);
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="command">Order creation data</param>
        /// <returns>Created order ID</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateOrder(CreateOrderCommand command)
        {
            var result = await Mediator.Send(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetOrder), new { id = result.Value }, new { id = result.Value });
            }

            return HandleResult(result);
        }

        /// <summary>
        /// Confirm an order
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Success result</returns>
        [HttpPost("{id:guid}/confirm")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> ConfirmOrder(Guid id)
        {
            var result = await Mediator.Send(new ConfirmOrderCommand(id));
            return HandleResult(result);
        }

        /// <summary>
        /// Update order status
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <param name="command">Status update data</param>
        /// <returns>Success result</returns>
        [HttpPut("{id:guid}/status")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateOrderStatus(Guid id, UpdateOrderStatusCommand command)
        {
            if (id != command.OrderId)
            {
                return BadRequest(new { error = "Order ID in URL does not match request body" });
            }

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
    }
}
