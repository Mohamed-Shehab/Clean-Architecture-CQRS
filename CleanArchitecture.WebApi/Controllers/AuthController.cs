using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Account.Commands.RegenerateTwoFactorRecoveryCodes;
using CleanArchitecture.Application.Features.Authentication.Commands.Login;
using CleanArchitecture.Application.Features.Authentication.Commands.Logout;
using CleanArchitecture.Application.Features.Authentication.Commands.LogoutAllSessions;
using CleanArchitecture.Application.Features.Authentication.Commands.LogoutSession;
using CleanArchitecture.Application.Features.Authentication.Commands.RefreshToken;
using CleanArchitecture.Application.Features.Authentication.Commands.Register;
using CleanArchitecture.Application.Features.Authentication.Commands.ValidateTwoFactor;
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
        /// <remarks>
        /// **Returns:**
        /// - **Standard Flow:** Returns full authentication tokens (Access Token, Refresh Token) and expiration details if 2FA is disabled.
        /// - **2FA Flow:** Returns a short-lived, single-purpose **Pre-Authentication Token** (`purpose: "2fa"`) required to call the verification endpoint.
        /// </remarks>
        /// 
        /// <param name="command">
        /// User login information.
        /// </param>
        /// 
        /// <returns>
        /// Returns either authentication tokens when login is completed successfully,
        /// or a pre-authentication token when two-factor authentication is required.
        /// </returns>
        /// 
        /// <response code="200">
        /// User logged in successfully or two-factor authentication is required.
        /// </response>
        /// <response code="400">
        /// Validation failed.
        /// </response>
        /// <response code="401">
        /// Authentication failed due to invalid credentials, or an unconfirmed email.
        /// </response>
        /// <response code="423">
        /// User account is temporarily locked.
        /// </response>

        [HttpPost("login")]
        [SwaggerRequestExample(typeof(LoginCommand), typeof(LoginCommandExample))]
        [ProducesResponseType<Response<LoginResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status423Locked)]

        public async Task<IActionResult> Login(LoginCommand command,
                                               CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Validate Two-Factor Authentication
        /// <summary>
        /// Validates the two-factor authentication code for the currently authenticating user
        /// and completes the authentication process.
        /// </summary>
        ///
        /// <remarks>
        /// This endpoint is called after a successful email and password authentication
        /// when two-factor authentication is enabled for the user.
        ///
        /// The authentication flow is:
        /// - The user first authenticates using their email and password.
        /// - If two-factor authentication is enabled, the login endpoint returns a
        ///   short-lived pre-authentication token.
        /// - The client sends that token as a Bearer token when calling this endpoint.
        /// - The provided authenticator code is validated.
        /// - If the code is valid, the login process is completed.
        /// - Access and refresh tokens are generated.
        ///
        /// **Important:**
        /// - This endpoint requires a valid two-factor pre-authentication token.
        /// - The pre-authentication token does not grant normal application access.
        /// - The verification code must be a valid 6-digit code generated by the
        ///   user's authenticator application.
        /// - If the verification code is invalid, the remaining verification attempts
        ///   are returned in the response metadata.
        /// - After the maximum number of failed attempts is reached, two-factor
        ///   verification is temporarily locked.
        /// </remarks>
        ///
        /// <returns>
        /// Returns access and refresh tokens after successful two-factor authentication
        /// verification.
        /// </returns>
        ///
        /// <response code="200">
        /// Two-factor authentication was successfully verified and the user was
        /// authenticated.
        /// </response>
        /// <response code="400">
        /// The provided verification code does not satisfy the required validation rules.
        /// </response>
        /// <response code="401">
        /// The provided two-factor authentication code is invalid, or the
        /// authentication pre-authentication token is invalid or missing.
        /// </response>
        /// <response code="423">
        /// Two-factor authentication verification is temporarily locked.
        /// </response>
        /// <response code="404">
        /// The authenticated user's account could not be found.
        /// </response>
        
        [HttpPost("2fa/validate")]
        [Authorize(Policy = "TwoFactorPreAuth")]
        [ProducesResponseType<Response<TokenResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status423Locked)]

        public async Task<IActionResult> ValidateTwoFactor(ValidateTwoFactorCommand request,
                                                           CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);


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
        [ProducesResponseType<Response<TokenResponse>>(StatusCodes.Status200OK)]
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
