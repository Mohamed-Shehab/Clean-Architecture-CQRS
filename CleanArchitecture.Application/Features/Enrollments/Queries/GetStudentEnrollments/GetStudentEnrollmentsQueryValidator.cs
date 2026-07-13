using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Enrollments.Queries.GetStudentEnrollments
{
    public sealed class GetStudentEnrollmentsQueryValidator : AbstractValidator<GetStudentEnrollmentsQuery>
    {
        public GetStudentEnrollmentsQueryValidator(IStringLocalizer<SharedResources> localizer)
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0)
                .WithMessage(localizer[ValidationErrors.GreaterThan, localizer[Fields.Id, localizer[Entities.Student]], 0]);
        }
    }
}
