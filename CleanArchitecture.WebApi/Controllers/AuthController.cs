using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Authentication.Commands.Login;
using CleanArchitecture.Application.Features.Authentication.Commands.RefreshToken;
using CleanArchitecture.Application.Features.Authentication.Commands.Register;
using CleanArchitecture.Application.Features.Authentication.Models;
using CleanArchitecture.WebApi.Controllers.Base;
using CleanArchitecture.WebApi.Swagger.Examples.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace CleanArchitecture.WebApi.Controllers
{
    /// <summary>
    /// Provides endpoints for user authentication, including registration and login.
    /// </summary>
    [Route("api/[controller]")]

    public class AuthController : BaseApiController
    {
        public AuthController(IMediator mediator) : base(mediator)
        {
        }


        #region Register
        /// <summary>
        /// Registers a new student.
        /// </summary>
        /// 
        /// <remarks>
        /// Validation Rules:
        ///
        /// - Email address must be unique.
        /// - Phone number must be unique.
        /// - Password must satisfy the configured Identity password policy.
        /// </remarks>
        /// 
        /// <param name="command">
        /// Registration information for the new student.
        /// </param>
        /// 
        /// <response code="201">
        /// Student registered successfully.
        /// </response>
        /// <response code="400">
        /// Validation failed.
        /// </response>
        /// <response code="409">
        /// The email/phone number is already in use.
        /// </response>

        [HttpPost("register")]
        [SwaggerRequestExample(typeof(RegisterCommand), typeof(RegisterCommandExample))]
        [ProducesResponseType<Response<object>>(StatusCodes.Status201Created)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status409Conflict)]

        public async Task<IActionResult> Register(RegisterCommand command,
                                                  CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Login
        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// 
        /// <param name="command">
        /// User login information.
        /// </param>
        /// 
        /// <returns>
        /// Returns the authentication tokens and their expiration information.
        /// </returns>
        /// 
        /// <response code="200">
        /// User logged in successfully.
        /// </response>
        /// <response code="400">
        /// Validation failed.
        /// </response>
        /// <response code="401">
        /// Authentication failed due to invalid credentials, an unconfirmed email, or a locked account.
        /// </response>

        [HttpPost("login")]
        [SwaggerRequestExample(typeof(LoginCommand), typeof(LoginCommandExample))]
        [ProducesResponseType<Response<AuthenticationResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> Login(LoginCommand command,
                                               CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Refresh Token
        /// <summary>
        /// Refreshes the authentication tokens using a valid refresh token.
        /// </summary>
        /// 
        /// <param name="command">
        /// Refresh token information.
        /// </param>
        /// 
        /// <returns>
        /// Returns new authentication tokens and their expiration information.
        /// </returns>
        /// 
        /// <response code="200">
        /// Authentication tokens refreshed successfully.
        /// </response>
        /// <response code="400">
        /// Validation failed.
        /// </response>
        /// <response code="401">
        /// Refresh token is invalid, expired, revoked, or associated with an invalid user.
        /// </response>

        [HttpPost("refresh-token")]
        [ProducesResponseType<Response<AuthenticationResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> RefreshToken(RefreshTokenCommand command,
                                                      CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);


            return HandleResponse(response);
        }
        #endregion
    }
}
