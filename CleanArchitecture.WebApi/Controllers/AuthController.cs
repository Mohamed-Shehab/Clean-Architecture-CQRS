using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Authentication.Commands.Login;
using CleanArchitecture.Application.Features.Authentication.Commands.Logout;
using CleanArchitecture.Application.Features.Authentication.Commands.LogoutAllSessions;
using CleanArchitecture.Application.Features.Authentication.Commands.LogoutSession;
using CleanArchitecture.Application.Features.Authentication.Commands.RefreshToken;
using CleanArchitecture.Application.Features.Authentication.Commands.Register;
using CleanArchitecture.Application.Features.Authentication.Models;
using CleanArchitecture.Application.Features.Authentication.Queries.GetUserSessions;
using CleanArchitecture.WebApi.Controllers.Base;
using CleanArchitecture.WebApi.Swagger.Examples.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace CleanArchitecture.WebApi.Controllers
{
    /// <summary>
    /// Provides endpoints for user authentication and session management.
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


        #region Get User Sessions
        /// <summary>
        /// Retrieves all active sessions for the currently authenticated user.
        /// </summary>
        /// 
        /// <returns>
        /// Returns the active sessions associated with the authenticated user,
        /// including device, location, and last activity information.
        /// </returns>
        /// 
        /// <response code="200">
        /// Active user sessions retrieved successfully.
        /// </response>
        /// <response code="401">
        /// User is not authenticated or the access token is invalid or expired.
        /// </response>

        [Authorize]
        [HttpGet("sessions")]
        [ProducesResponseType<Response<List<UserSessionDto>>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> GetUserSessions(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetUserSessionsQuery(), cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Logout
        /// <summary>
        /// Logs out the user from the current session.
        /// </summary>
        ///
        /// <remarks>
        /// If the provided refresh token does not belong to the authenticated user,
        /// or the session is already revoked or does not exist, the operation still
        /// returns a successful response to keep the logout operation idempotent.
        /// </remarks>
        ///
        /// <param name="command">
        /// Refresh token information for the current session.
        /// </param>
        ///
        /// <returns>
        /// Returns a successful response after the current session has been logged out.
        /// </returns>
        ///
        /// <response code="200">
        /// User logged out successfully from the current session.
        /// </response>
        /// <response code="400">
        /// Validation failed.
        /// </response>
        /// <response code="401">
        /// User is not authenticated or the access token is invalid or expired.
        /// </response>

        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType<Response<object>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> Logout(LogoutCommand command, 
                                                CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Logout Session
        /// <summary>
        /// Logs out the user from a specific session.
        /// </summary>
        /// 
        /// <remarks>
        /// The specified session must belong to the currently authenticated user.
        /// If the session does not exist, belongs to another user, or is already revoked,
        /// the operation still returns a successful response to keep the logout operation idempotent.
        /// </remarks>
        /// 
        /// <param name="command">
        /// Information identifying the session to be logged out.
        /// </param>
        /// 
        /// <returns>
        /// Returns a successful response after the specified session has been logged out.
        /// </returns>
        /// 
        /// <response code="200">
        /// User logged out successfully from the specified session.
        /// </response>
        /// <response code="401">
        /// User is not authenticated or the access token is invalid or expired.
        /// </response>

        [Authorize]
        [HttpPost("logout-session")]
        [ProducesResponseType<Response<object>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> LogoutSession(LogoutSessionCommand command, 
                                                       CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Logout All Sessions
        /// <summary>
        /// Logs out the user from all active sessions across all devices.
        /// </summary>
        ///
        /// <remarks>
        /// All active sessions associated with the currently authenticated user are revoked.
        /// If the user has no active sessions, the operation still returns a successful response.
        /// </remarks>
        ///
        /// <returns>
        /// Returns a successful response after all active sessions have been logged out.
        /// </returns>
        ///
        /// <response code="200">
        /// User logged out successfully from all sessions.
        /// </response>
        /// <response code="401">
        /// User is not authenticated or the access token is invalid or expired.
        /// </response>

        [Authorize]
        [HttpPost("logout-all")]
        [ProducesResponseType<Response<object>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> LogoutAllSessions(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new LogoutAllSessionsCommand(), cancellationToken);


            return HandleResponse(response);
        }
        #endregion

    }
}
