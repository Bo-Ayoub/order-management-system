using Azure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Common.Models;

namespace OrderManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class BaseApiController : ControllerBase
    {
        private ISender? _mediator;
        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

        protected ActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return result.Value != null ? Ok(result.Value) : NotFound();
            }

            return BadRequest(new { error = result.Error });
        }

        protected ActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(new { error = result.Error });
        }

        protected ActionResult HandlePaginatedResult<T>(Result<PaginatedList<T>> result)
        {
            if (result.IsSuccess && result.Value != null)
            {
                var paginatedList = result.Value;

                // Add pagination headers
                Response.Headers.Append("X-Pagination-PageNumber", paginatedList.PageNumber.ToString());
                Response.Headers.Append("X-Pagination-TotalPages", paginatedList.TotalPages.ToString());
                Response.Headers.Append("X-Pagination-TotalCount", paginatedList.TotalCount.ToString());
                Response.Headers.Append("X-Pagination-HasNext", paginatedList.HasNextPage.ToString());
                Response.Headers.Append("X-Pagination-HasPrevious", paginatedList.HasPreviousPage.ToString());

                return Ok(new
                {
                    data = paginatedList.Items,
                    pagination = new
                    {
                        pageNumber = paginatedList.PageNumber,
                        totalPages = paginatedList.TotalPages,
                        totalCount = paginatedList.TotalCount,
                        hasNextPage = paginatedList.HasNextPage,
                        hasPreviousPage = paginatedList.HasPreviousPage
                    }
                });
            }

            return BadRequest(new { error = result.Error });
        }
    }
}
