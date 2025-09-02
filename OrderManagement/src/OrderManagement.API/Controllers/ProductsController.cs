using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Features.Products.Commands.CreateProduct;
using OrderManagement.Application.Features.Products.Queries.GetProduct;

namespace OrderManagement.API.Controllers
{
    /// <summary>
    /// Product management endpoints
    /// </summary>
    [Route("api/[controller]")]
    public class ProductsController : BaseApiController
    {
        /// <summary>
        /// Get all products with optional filtering and pagination
        /// </summary>
        /// <param name="query">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of products</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetProducts([FromQuery] GetProductsQuery query)
        {
            var result = await Mediator.Send(query);
            return HandlePaginatedResult(result);
        }

        /// <summary>
        /// Get a specific product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetProduct(Guid id)
        {
            var result = await Mediator.Send(new GetProductQuery(id));
            return HandleResult(result);
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="command">Product creation data</param>
        /// <returns>Created product ID</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateProduct(CreateProductCommand command)
        {
            var result = await Mediator.Send(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetProduct), new { id = result.Value }, new { id = result.Value });
            }

            return HandleResult(result);
        }
    }
}
