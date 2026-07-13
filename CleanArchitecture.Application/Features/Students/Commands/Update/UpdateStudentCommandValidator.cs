using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Students.Commands.Update
{
    public class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
    {
        private readonly IStringLocalizer<SharedResources> _localizer;

        public UpdateStudentCommandValidator(IStringLocalizer<SharedResources>localizer)
        {
            this._localizer = localizer;

            ValidateId();
            ValidateFirstName();
            ValidateLastName();
            ValidatePhoneNumber();
            ValidateDateOfBirth();
            ValidateAddress();
        }

        private void ValidateId()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage(_localizer[ValidationErrors.GreaterThan, _localizer[Fields.Id, _localizer[Entities.Student]], 0]);
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

        private void ValidatePhoneNumber()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.PhoneNumber]]);

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage(_localizer[ValidationErrors.MaxLength, _localizer[Fields.PhoneNumber], 20])
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
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
