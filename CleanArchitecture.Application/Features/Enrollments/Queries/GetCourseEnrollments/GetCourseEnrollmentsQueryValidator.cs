using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Enrollments.Queries.GetCourseEnrollments
{
    public sealed class GetCourseEnrollmentsQueryValidator : AbstractValidator<GetCourseEnrollmentsQuery>
    {
        public GetCourseEnrollmentsQueryValidator(IStringLocalizer<SharedResources> localizer)
        {
            RuleFor(x => x.CourseId)
                .GreaterThan(0)
                .WithMessage(localizer[ValidationErrors.GreaterThan, localizer[Fields.Id, localizer[Entities.Course]], 0]);
        }
    }
}
