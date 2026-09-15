using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Account.Commands.DisableTwoFactor
{
    public sealed class DisableTwoFactorCommandValidator : AbstractValidator<DisableTwoFactorCommand>
    {
        private readonly IStringLocalizer<SharedResources> _localizer;


        public DisableTwoFactorCommandValidator(IStringLocalizer<SharedResources> localizer)
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
