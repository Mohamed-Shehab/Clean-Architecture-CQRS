using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Authentication.Commands.ValidateTwoFactorRecoveryCode
{
    public sealed class ValidateTwoFactorRecoveryCodeCommandValidator : AbstractValidator<ValidateTwoFactorRecoveryCodeCommand>
    {
        private readonly IStringLocalizer<SharedResources> _localizer;


        public ValidateTwoFactorRecoveryCodeCommandValidator(IStringLocalizer<SharedResources> localizer)
        {
            this._localizer = localizer;


            RuleFor(x => x.RecoveryCode)
                .NotEmpty()
                .WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.RecoveryCode]]);
        }
    }
}
