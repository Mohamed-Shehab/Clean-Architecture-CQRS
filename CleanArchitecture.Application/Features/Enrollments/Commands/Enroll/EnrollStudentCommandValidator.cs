using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Enrollments.Commands.Enroll
{
    public sealed class EnrollStudentCommandValidator : AbstractValidator<EnrollStudentCommand>
    {
        public EnrollStudentCommandValidator(IStringLocalizer<SharedResources> localizer)
        {
            RuleFor(x => x.StudentId)
               .GreaterThan(0)
               .WithMessage(localizer[ValidationErrors.GreaterThan, localizer[Fields.Id, localizer[Entities.Student]], 0]);


            RuleFor(x => x.CourseId)
                .GreaterThan(0)
                .WithMessage(localizer[ValidationErrors.GreaterThan, localizer[Fields.Id, localizer[Entities.Course]], 0]);
        }
    }
}
