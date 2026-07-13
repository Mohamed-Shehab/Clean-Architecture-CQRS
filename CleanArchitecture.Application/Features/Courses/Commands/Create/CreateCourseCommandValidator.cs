using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Courses.Commands.Create
{
    public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        private readonly IStringLocalizer<SharedResources> _localizer;

        public CreateCourseCommandValidator(IStringLocalizer<SharedResources> localizer)
        {
            this._localizer = localizer;

            ValidateNameEn();
            ValidateNameAr();
            ValidateDescription();
            ValidateCapacity();
        }

        private void ValidateNameEn()
        {
            RuleFor(x => x.NameEn)
                .NotEmpty()
                .WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.NameEn, _localizer[Entities.Course]]]);


            RuleFor(x => x.NameEn)
                .MinimumLength(2)
                .WithMessage(_localizer[ValidationErrors.MinLength, _localizer[Fields.NameEn, _localizer[Entities.Course], 2]])
                .MaximumLength(150)
                .WithMessage(_localizer[ValidationErrors.MaxLength, _localizer[Fields.NameEn, _localizer[Entities.Course]], 150])
                .When(x => !string.IsNullOrWhiteSpace(x.NameEn));
        }

        private void ValidateNameAr()
        {
            RuleFor(x => x.NameAr)
                .NotEmpty()
                .WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.NameAr, _localizer[Entities.Course]]]);


            RuleFor(x => x.NameAr)
                .MinimumLength(2)
                .WithMessage(_localizer[ValidationErrors.MinLength, _localizer[Fields.NameAr, _localizer[Entities.Course], 2]])
                .MaximumLength(150)
                .WithMessage(_localizer[ValidationErrors.MaxLength, _localizer[Fields.NameAr, _localizer[Entities.Course]], 150])
                .When(x => !string.IsNullOrWhiteSpace(x.NameAr));
        }

        private void ValidateDescription()
        {
            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage(_localizer[ValidationErrors.MaxLength, _localizer[Fields.Description, _localizer[Entities.Course]], 1000])
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }

        private void ValidateCapacity()
        {
            RuleFor(x => x.Capacity)
                .GreaterThan(0)
                .WithMessage(_localizer[ValidationErrors.MinValue, _localizer[Fields.Capacity, _localizer[Entities.Course]], 1]);
        }
        
    }
}
