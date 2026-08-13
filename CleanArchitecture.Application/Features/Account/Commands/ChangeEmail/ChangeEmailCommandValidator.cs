using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Account.Commands.ChangeEmail
{
    public sealed class ChangeEmailCommandValidator : AbstractValidator<ChangeEmailCommand>
    {
        private readonly IStringLocalizer<SharedResources> _localizer;


        public ChangeEmailCommandValidator(IStringLocalizer<SharedResources> localizer)
        {
            this._localizer = localizer;

            ValidatePassword();
            ValidateNewEmail();
        }


        private void ValidatePassword()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.Password]]);
        }

        private void ValidateNewEmail()
        {
            RuleFor(x => x.NewEmail)
                .NotEmpty()
                .WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.Email]])
                .EmailAddress()
                .WithMessage(_localizer[ValidationErrors.InvalidEmail, _localizer[Fields.Email]]);
        }
    }
}
