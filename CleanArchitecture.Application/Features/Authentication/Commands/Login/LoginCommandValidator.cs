using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Authentication.Commands.Login
{
    public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        private readonly IStringLocalizer<SharedResources> _localizer;


        public LoginCommandValidator(IStringLocalizer<SharedResources> localizer)
        {
            this._localizer = localizer;

            ValidateEmail();
            ValidatePassword();
        }


        private void ValidateEmail()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.Email]])
                .EmailAddress()
                .WithMessage(_localizer[ValidationErrors.InvalidEmail, _localizer[Fields.Email]]);
        }

        private void ValidatePassword()
        {
            RuleFor(x => x.Password)
               .NotEmpty()
               .WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.Password]]);
        }
    }
}
