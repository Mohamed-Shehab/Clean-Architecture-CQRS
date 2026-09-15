using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Authentication.Commands.ValidateTwoFactor
{
    public sealed class ValidateTwoFactorCommandValidator : AbstractValidator<ValidateTwoFactorCommand>
    {
        private readonly IStringLocalizer<SharedResources> _localizer;


        public ValidateTwoFactorCommandValidator(IStringLocalizer<SharedResources> localizer)
        {
            this._localizer = localizer;


            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage(_localizer[ValidationErrors.Required, Fields.VerificationCode]);

            RuleFor(x => x.Code)
                .Length(6)
                .When(x => !string.IsNullOrWhiteSpace(x.Code))
                .WithMessage(_localizer[ValidationErrors.ExactLength, _localizer[Fields.VerificationCode], 6]);

            RuleFor(x => x.Code)
                .Matches("^[0-9]+$")
                .When(x => !string.IsNullOrWhiteSpace(x.Code))
                .WithMessage(_localizer[ValidationErrors.DigitsOnly, _localizer[Fields.VerificationCode]]);
        }
    }
}
