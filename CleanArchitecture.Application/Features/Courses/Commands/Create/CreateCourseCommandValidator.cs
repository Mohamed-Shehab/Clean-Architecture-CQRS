using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Domain.Interfaces.Repositories;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Courses.Commands.Create
{
    public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public CreateCourseCommandValidator(ICourseRepository courseRepository, IStringLocalizer<SharedResources> localizer)
        {
            this._courseRepository = courseRepository;
            this._localizer = localizer;

            ValidateTitle();
            ValidateMaxStudents();
        }

        private void ValidateTitle()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.Title]]);


            RuleFor(x => x.Title)
                .MinimumLength(2).WithMessage(_localizer[ValidationErrors.MinLength, _localizer[Fields.Title], 2])
                .MaximumLength(100).WithMessage(_localizer[ValidationErrors.MaxLength, _localizer[Fields.Title], 100])
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.Title)
                .MustAsync(BeUniqueTitle).WithMessage(_localizer[ValidationErrors.AlreadyExists, _localizer[Fields.Title]]);
        }

        private void ValidateMaxStudents()
        {
            RuleFor(x => x.MaxStudents)
                .GreaterThanOrEqualTo(1).WithMessage(_localizer[ValidationErrors.MinValue, _localizer[Fields.MaxStudents], 1])
                .LessThanOrEqualTo(1000).WithMessage(_localizer[ValidationErrors.MaxValue, _localizer[Fields.MaxStudents], 1000]);
        }
        
        private async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken)
        {
            return !await _courseRepository.AnyAsync(c => c.Title == title, cancellationToken);
        }
    }
}
