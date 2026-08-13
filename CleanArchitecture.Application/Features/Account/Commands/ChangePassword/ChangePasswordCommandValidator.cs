using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Account.Commands.ChangePassword
{
    public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        private readonly IStringLocalizer<SharedResources> _localizer;


        public ChangePasswordCommandValidator(IStringLocalizer<SharedResources> localizer)
        {
            this._localizer = localizer;

            ValidateCurrentPassword();
            ValidateNewPassword();
            ValidateConfirmNewPassword();
        }


        private void ValidateCurrentPassword()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.CurrentPassword]]);
        }

        private void ValidateNewPassword()
        {
            RuleFor(x => x.NewPassword)
               .NotEmpty()
               .WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.NewPassword]]);

            RuleFor(x => x.NewPassword)
                .MinimumLength(8)
                .WithMessage(_localizer[ValidationErrors.MinLength, _localizer[Fields.NewPassword], 8])
                .When(x => !string.IsNullOrEmpty(x.NewPassword));
        }

        private void ValidateConfirmNewPassword()
        {
            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty()
                .WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.ConfirmNewPassword]]);

            RuleFor(x => x.ConfirmNewPassword)
                .Equal(x => x.NewPassword)
                .WithMessage(_localizer[ValidationErrors.PasswordsDoNotMatch])
                .When(x => !string.IsNullOrEmpty(x.ConfirmNewPassword));
        }
    }
}
