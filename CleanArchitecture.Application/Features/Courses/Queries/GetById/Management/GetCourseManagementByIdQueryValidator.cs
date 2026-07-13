using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Courses.Queries.GetById.Management
{
    public sealed class GetCourseManagementByIdQueryValidator : AbstractValidator<GetCourseManagementByIdQuery>
    {
        public GetCourseManagementByIdQueryValidator(IStringLocalizer<SharedResources> localizer)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage(localizer[ValidationErrors.GreaterThan, localizer[Fields.Id, localizer[Entities.Course]], 0]);
        }
    }
}
