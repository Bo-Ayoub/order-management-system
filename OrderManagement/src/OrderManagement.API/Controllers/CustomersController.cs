using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Features.Customers.Commands.CreateCustomer;
using OrderManagement.Application.Features.Customers.Queries.GetCustomer;
using OrderManagement.Application.Features.Customers.Queries.GetCustomers;

namespace OrderManagement.API.Controllers
{
    /// <summary>
    /// Customer management endpoints
    /// </summary>
    [Route("api/[controller]")]
    public class CustomersController : BaseApiController
    {
        /// <summary>
        /// Get all customers with optional search and pagination
        /// </summary>
        /// <param name="query">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of customers</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetCustomers([FromQuery] GetCustomersQuery query)
        {
            var result = await Mediator.Send(query);
            return HandlePaginatedResult(result);
        }

        /// <summary>
        /// Get a specific customer by ID
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>Customer details</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetCustomer(Guid id)
        {
            var result = await Mediator.Send(new GetCustomerQuery(id));
            return HandleResult(result);
        }


        /// <summary>
        /// Create a new customer
        /// </summary>
        /// <param name="command">Customer creation data</param>
        /// <returns>Created customer ID</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateCustomer(CreateCustomerCommand command)
        {
            var result = await Mediator.Send(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetCustomer), new { id = result.Value }, new { id = result.Value });
            }

            return HandleResult(result);
        }
    }
}
