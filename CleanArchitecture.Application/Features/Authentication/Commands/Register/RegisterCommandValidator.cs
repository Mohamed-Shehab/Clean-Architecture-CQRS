using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Authentication.Commands.Register
{
    public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        private readonly IStringLocalizer<SharedResources> _localizer;


        public RegisterCommandValidator(IStringLocalizer<SharedResources> localizer)
        {
            this._localizer = localizer;

            ValidateFirstName();
            ValidateLastName();
            ValidateEmail();
            ValidatePhoneNumber();
            ValidatePassword();
            ValidateDateOfBirth();
            ValidateAddress();
        }


        private void ValidateFirstName()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.FirstName]]);

            RuleFor(x => x.FirstName)
            .MinimumLength(2).WithMessage(_localizer[ValidationErrors.MinLength, _localizer[Fields.FirstName], 2])
            .MaximumLength(50).WithMessage(_localizer[ValidationErrors.MaxLength, _localizer[Fields.FirstName], 50])
            .When(x => !string.IsNullOrWhiteSpace(x.FirstName));
        }

        private void ValidateLastName()
        {
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.LastName]]);

            RuleFor(x => x.LastName)
            .MinimumLength(2).WithMessage(_localizer[ValidationErrors.MinLength, _localizer[Fields.LastName], 2])
            .MaximumLength(50).WithMessage(_localizer[ValidationErrors.MaxLength, _localizer[Fields.LastName], 50])
            .When(x => !string.IsNullOrWhiteSpace(x.LastName));
        }

        private void ValidateEmail()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.Email]])
                .EmailAddress().WithMessage(_localizer[ValidationErrors.InvalidEmail, _localizer[Fields.Email]]);
        }

        private void ValidatePhoneNumber()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.PhoneNumber]]);

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage(_localizer[ValidationErrors.MaxLength, _localizer[Fields.PhoneNumber], 20])
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
        }

        private void ValidatePassword()
        {
            RuleFor(x => x.Password)
               .NotEmpty().WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.Password]]);

            RuleFor(x => x.Password)
                .MinimumLength(8).WithMessage(_localizer[ValidationErrors.MinLength, _localizer[Fields.Password], 8])
                .When(x => !string.IsNullOrEmpty(x.Password));
        }

        private void ValidateDateOfBirth()
        {
            RuleFor(x => x.DateOfBirth)
                .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage(_localizer[ValidationErrors.InvalidValue, _localizer[Fields.DateOfBirth]]);
        }

        private void ValidateAddress()
        {
            RuleFor(x => x.Address)
                .MaximumLength(250)
                .WithMessage(_localizer[ValidationErrors.MaxLength, _localizer[Fields.Address], 250])
                .When(x => !string.IsNullOrWhiteSpace(x.Address));
        }

    }
}
