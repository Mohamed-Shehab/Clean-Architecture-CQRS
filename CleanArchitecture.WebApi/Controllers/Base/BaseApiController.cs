using CleanArchitecture.Application.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebApi.Controllers.Base
{
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly IMediator _mediator;

        protected BaseApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected IActionResult HandleResponse<T>(Response<T> response)
        {
            if (response == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            return response.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(response),

                StatusCodes.Status201Created =>
                    StatusCode(StatusCodes.Status201Created, response),

                StatusCodes.Status204NoContent =>
                    NoContent(),

                StatusCodes.Status400BadRequest =>
                    BadRequest(response),

                StatusCodes.Status404NotFound =>
                    NotFound(response),

                StatusCodes.Status401Unauthorized =>
                    Unauthorized(response),

                _ => StatusCode(response.StatusCode, response)
            };
        }
    }
}
