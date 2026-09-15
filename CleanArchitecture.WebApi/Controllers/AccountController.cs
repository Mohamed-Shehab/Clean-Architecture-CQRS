using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Account.Commands.ChangeEmail;
using CleanArchitecture.Application.Features.Account.Commands.ChangePassword;
using CleanArchitecture.Application.Features.Account.Commands.ConfirmTwoFactor;
using CleanArchitecture.Application.Features.Account.Commands.DisableTwoFactor;
using CleanArchitecture.Application.Features.Account.Commands.RegenerateTwoFactorRecoveryCodes;
using CleanArchitecture.Application.Features.Account.Commands.SetupTwoFactor;
using CleanArchitecture.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        /// <response code="401">
        /// The user is not authenticated.
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
        /// <response code="409">
        /// The new email address is already in use.
        /// </response>

        [HttpPut("email")]
        [ProducesResponseType<Response<object>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status409Conflict)]

        public async Task<IActionResult> ChangeEmail(ChangeEmailCommand command,
                                                     CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Setup Two-Factor Authentication
        /// <summary>
        /// Initiates two-factor authentication setup for the currently authenticated user.
        /// </summary>
        /// 
        /// <remarks>
        /// Generates a new authenticator key and returns a QR code that can be scanned
        /// using an authenticator application such as Microsoft Authenticator or Google Authenticator.
        /// 
        /// **Important:**
        /// - This endpoint does not enable two-factor authentication.
        /// - The returned QR code must be scanned and the generated verification code
        ///   must be confirmed through the two-factor authentication confirmation endpoint.
        /// - A new authenticator key is generated when this endpoint is called.
        /// </remarks>
        /// 
        /// <returns>
        /// Returns a QR code image that can be scanned by an authenticator application.
        /// </returns>
        /// 
        /// <response code="200">
        /// Two-factor authentication setup was initiated successfully and the QR code was generated.
        /// </response>
        /// <response code="401">
        /// The user is not authenticated.
        /// </response>
        /// <response code="404">
        /// The authenticated user's account could not be found.
        /// </response>

        [HttpPost("2fa/setup")]
        [ProducesResponseType<Response<SetupTwoFactorResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> SetupTwoFactor(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new SetupTwoFactorCommand(), cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Confirm Two-Factor Authentication
        /// <summary>
        /// Confirms two-factor authentication setup for the currently authenticated user.
        /// </summary>
        ///
        /// <remarks>
        /// Verifies the authenticator application code provided by the user.
        /// If the code is valid, two-factor authentication is enabled and a new set
        /// of recovery codes is generated.
        ///
        /// **Important:**
        /// - The authenticator application must be configured before calling this endpoint.
        /// - The verification code must be a valid 6-digit code generated by the authenticator application.
        /// - Two-factor authentication is enabled only after successful verification.
        /// - Recovery codes are generated after successful activation and should be stored securely.
        /// - If the verification code is invalid, the remaining verification attempts are returned in the response metadata.
        /// - After the maximum number of failed attempts is reached, two-factor authentication verification is temporarily locked.
        /// </remarks>
        ///
        /// <returns>
        /// Returns the generated recovery codes after successful two-factor authentication activation.
        /// </returns>
        /// <response code="200">
        /// Two-factor authentication was enabled successfully and recovery codes were generated.
        /// </response>
        /// <response code="400">
        /// Validation failed.
        /// </response>
        /// <response code="401">
        /// The provided two-factor authentication code is invalid.
        /// </response>
        /// <response code="423">
        /// Two-factor authentication verification is temporarily locked.
        /// </response>

        [HttpPost("2fa/confirm")]
        [ProducesResponseType<Response<ConfirmTwoFactorResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status423Locked)]

        public async Task<IActionResult> ConfirmTwoFactor(ConfirmTwoFactorCommand request,
                                                          CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Regenerate Two-Factor Recovery Codes
        /// <summary>
        /// Generates a new set of two-factor authentication recovery codes
        /// for the currently authenticated user.
        /// </summary>
        /// 
        /// <remarks>
        /// This endpoint allows the authenticated user to replace their existing
        /// two-factor authentication recovery codes.
        ///
        /// The authentication flow is:
        /// - The user must be authenticated.
        /// - The user must provide a valid six-digit TOTP code from their authenticator application.
        /// - The TOTP code is verified before the recovery codes are regenerated.
        /// - A new set of recovery codes is generated.
        /// - All previously generated recovery codes become invalid.
        ///
        /// <b>Important:</b>
        /// The recovery codes are sensitive credentials and should be stored securely.
        /// The user should save the newly generated recovery codes in a secure location.
        /// </remarks>
        /// 
        /// <returns>
        /// Returns the newly generated two-factor authentication recovery codes.
        /// </returns>
        /// 
        /// <response code="200">
        /// New two-factor authentication recovery codes were generated successfully.
        /// </response>
        /// <response code="400">
        /// The provided verification code does not satisfy the required validation rules.
        /// </response>
        /// <response code="401">
        /// The provided two-factor authentication code is invalid.
        /// </response>
        /// <response code="423">
        /// Two-factor authentication verification is temporarily locked.
        /// </response>
        /// <response code="404">
        /// The authenticated user's account could not be found.
        /// </response>
        /// <response code="500">
        /// The recovery codes could not be generated due to an unexpected server error.
        /// </response>

        [HttpPost("2fa/recovery-codes/regenerate")]
        [ProducesResponseType<Response<RegenerateTwoFactorRecoveryCodesResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status423Locked)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> RegenerateTwoFactorRecoveryCodes(RegenerateTwoFactorRecoveryCodesCommand request,
                                                                          CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);


            return HandleResponse(response);
        }
        #endregion


        #region Disable Two-Factor Authentication
        /// <summary>
        /// Disables two-factor authentication for the currently authenticated user.
        /// </summary>
        ///
        /// <remarks>
        /// This endpoint allows the authenticated user to disable two-factor authentication
        /// after verifying their identity using a valid authenticator application code.
        ///
        /// The authentication flow is:
        /// - The user must be authenticated using a valid access token.
        /// - The TOTP code is verified before two-factor authentication is disabled.
        /// - If the verification succeeds, two-factor authentication is disabled for the user.
        ///
        /// <b>Important:</b>
        /// - A valid six-digit TOTP code is required to disable two-factor authentication.
        /// - If two-factor authentication is enabled again later, the authenticator setup should be completed and verified again.
        /// </remarks>
        ///
        /// <returns>
        /// Returns a successful response when two-factor authentication is disabled.
        /// </returns>
        ///
        /// <response code="200">
        /// Two-factor authentication was disabled successfully.
        /// </response>
        /// <response code="400">
        /// The provided verification code does not satisfy the required validation rules.
        /// </response>
        /// <response code="401">
        /// The provided two-factor authentication code is invalid, or the user is not authenticated.
        /// </response>
        /// <response code="423">
        /// Two-factor authentication verification is temporarily locked.
        /// </response>
        /// <response code="500">
        /// Two-factor authentication could not be disabled due to an unexpected server error.
        /// </response>

        [HttpPost("2fa/disable")]
        [ProducesResponseType<Response<object>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status423Locked)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DisableTwoFactor(DisableTwoFactorCommand request,
                                                          CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);


            return HandleResponse(response);
        }
        #endregion
    }
}
