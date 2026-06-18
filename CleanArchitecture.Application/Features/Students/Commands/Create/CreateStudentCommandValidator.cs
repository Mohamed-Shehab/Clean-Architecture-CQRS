using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Students.Commands.Create
{
    public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public CreateStudentCommandValidator(IStudentRepository studentRepository,
                                             IStringLocalizer<SharedResources> localizer)
        {
            this._studentRepository = studentRepository;
            this._localizer = localizer;

            ValidateName();
            ValidateEmail();
            ValidatePassword();
        }

        private void ValidateName()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.Name]]);


            RuleFor(x => x.Name)
                .MinimumLength(3).WithMessage(_localizer[ValidationErrors.MinLength, _localizer[Fields.Name], 3])
                .MaximumLength(100).WithMessage(_localizer[ValidationErrors.MaxLength, _localizer[Fields.Name], 150])
                .When(x => !string.IsNullOrEmpty(x.Name));
        }

        private void ValidateEmail()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.Email]])
                .EmailAddress().WithMessage(_localizer[ValidationErrors.InvalidEmail, _localizer[Fields.Email]])
                .MustAsync(BeUniqueEmail).WithMessage(_localizer[ValidationErrors.AlreadyUsed, _localizer[Fields.Email]]);
        }

        private void ValidatePassword()
        {
            RuleFor(x => x.Password)
               .NotEmpty().WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.Password]]);

            RuleFor(x => x.Password)
                .MinimumLength(6).WithMessage(_localizer[ValidationErrors.MinLength, _localizer[Fields.Password], 6])
                .When(x => !string.IsNullOrEmpty(x.Password));
        }

        private async Task<bool> BeUniqueEmail(
            string email,
            CancellationToken cancellationToken)
        {
            var exists = await _studentRepository.AnyAsync(s => s.Email == email, cancellationToken);

            return !exists;
        }

    }
}
