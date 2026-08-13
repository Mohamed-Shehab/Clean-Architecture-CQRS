using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Account.Commands.ChangeEmail;
using CleanArchitecture.Application.Features.Account.Commands.ChangePassword;
using CleanArchitecture.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace CleanArchitecture.WebApi.Controllers
{
    /// <summary>
    /// Provides endpoints for managing the authenticated user's account.
    /// </summary>
    
    [Authorize]
    [Route("api/[controller]")]

    public class AccountController : BaseApiController
    {
        public AccountController(IMediator mediator) : base(mediator)
        {
        }


        #region Change Password
        /// <summary>
        /// Changes the password of the authenticated user.
        /// </summary>
        /// 
        /// <remarks>
        /// The current password must be correct, and the new password must satisfy
        /// the configured password requirements.
        /// </remarks>
        /// 
        /// <param name="command">
        /// Current and new password information.
        /// </param>
        /// 
        /// <response code="200">
        /// Password changed successfully.
        /// </response>
        /// <response code="400">
        /// The current password is incorrect or the password change failed.
        /// </response>

        [HttpPut("password")]
        [ProducesResponseType<Response<object>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> ChangePassword(ChangePasswordCommand command, 
                                                        CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Change Email
        /// <summary>
        /// Changes the authenticated user's email address.
        /// </summary>
        /// 
        /// <remarks>
        /// Validation Rules:
        ///
        /// - The new email address must be valid.
        /// - The new email address must be different from the current email address.
        /// - The new email address must not already be in use.
        /// - The current password must be correct.
        /// 
        /// The new email address will require confirmation after the change.
        /// </remarks>
        /// 
        /// <param name="command">
        /// Email change information.
        /// </param>
        /// 
        /// <response code="200">
        /// Email address changed successfully.
        /// </response>
        /// <response code="400">
        /// Validation failed, the new email is the same as the current email,
        /// the current password is incorrect, the account is locked,
        /// or the email change failed.
        /// </response>
        /// <response code="401">
        /// The user is not authenticated.
        /// </response>
        /// <response code="404">
        /// The authenticated user was not found.
        /// </response>
        /// <response code="409">
        /// The new email address is already in use.
        /// </response>
        
        [HttpPut("email")]
        [ProducesResponseType<Response<object>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status409Conflict)]

        public async Task<IActionResult> ChangeEmail(ChangeEmailCommand command,
                                                     CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);


            return HandleResponse(response);
        }
        #endregion
    }
}
