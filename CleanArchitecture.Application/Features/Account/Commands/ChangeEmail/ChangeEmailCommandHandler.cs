using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.CurrentUser;
using CleanArchitecture.Application.Common.Services.Identity;
using CleanArchitecture.Application.Common.Services.Identity.Enums;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Account.Commands.ChangeEmail
{
    public sealed class ChangeEmailCommandHandler : IRequestHandler<ChangeEmailCommand, Response<object>>
    {
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public ChangeEmailCommandHandler(IIdentityService identityService,
                                         ICurrentUserService currentUserService,
                                         IStringLocalizer<SharedResources> localizer)
        {
            this._identityService = identityService;
            this._currentUserService = currentUserService;
            this._localizer = localizer;
        }


        public async Task<Response<object>> Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
        {
            var userEmail = _currentUserService.Email;

            if (string.Equals(userEmail, request.NewEmail, StringComparison.OrdinalIgnoreCase))
            {
                return ResponseHandler.BadRequest<object>(
                    _localizer[Errors.AlreadyUsed, _localizer[Fields.Email]],
                    errorCode: ErrorCodes.Account.EmailAlreadyUsed);
            }


            var isEmailUsed = await _identityService.IsEmailUsedAsync(request.NewEmail, cancellationToken);

            if (isEmailUsed)
            {
                return ResponseHandler.Conflict<object>(
                    _localizer[Errors.AlreadyUsed, _localizer[Fields.Email]],
                    errorCode: ErrorCodes.Account.EmailAlreadyUsed);
            }


            var userId = _currentUserService.UserId;

            var changeEmailResult = await _identityService.ChangeEmailAsync(
                userId,
                request.CurrentPassword, 
                request.NewEmail, 
                cancellationToken);


            if (!changeEmailResult.Succeeded)
            {
                return HandleChangeEmailFailure(changeEmailResult.FailureReason, changeEmailResult.Errors);
            } 


            return ResponseHandler.Success<object>(
                message: _localizer[Messages.EmailChangedSuccessfully]);
        }


        private Response<object> HandleChangeEmailFailure(ChangeEmailFailureReason failureReason, List<string>? errors)
        {
            return failureReason switch
            {
                ChangeEmailFailureReason.UserNotFound => ResponseHandler.NotFound<object>(
                    _localizer[Messages.NotFound, _localizer[Entities.User]]),


                ChangeEmailFailureReason.AccountLocked => ResponseHandler.BadRequest<object>(
                    _localizer[Errors.AccountLocked],
                    errorCode: ErrorCodes.Account.AccountLocked),


                ChangeEmailFailureReason.InvalidCurrentPassword => ResponseHandler.BadRequest<object>(
                    _localizer[Errors.InvalidPassword],
                    errorCode: ErrorCodes.Account.InvalidCurrentPassword),


                ChangeEmailFailureReason.ChangeEmailFailed => ResponseHandler.BadRequest<object>(
                    _localizer[Messages.ChangeEmailFailed],
                    errorCode: ErrorCodes.Account.ChangeEmailFailed,
                    errors: errors),


                _ => ResponseHandler.BadRequest<object>(
                    _localizer[Messages.ChangeEmailFailed],
                    errorCode: ErrorCodes.Account.ChangeEmailFailed,
                    errors: errors)
            };
        }
    }
}
