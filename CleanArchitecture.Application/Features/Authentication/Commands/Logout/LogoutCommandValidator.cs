using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Authentication.Commands.Logout
{
    public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        private readonly IStringLocalizer<SharedResources> _localizer;


        public LogoutCommandValidator(IStringLocalizer<SharedResources> localizer)
        {
            this._localizer = localizer;

            ValidateRefreshToken();
        }


        private void ValidateRefreshToken()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage(_localizer[ValidationErrors.Required, _localizer[Fields.RefreshToken]]);
        }
    }
}
