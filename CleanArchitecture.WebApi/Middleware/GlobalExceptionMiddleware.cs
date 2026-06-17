using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.WebApi.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public GlobalExceptionMiddleware(RequestDelegate next, IStringLocalizer<SharedResources> localizer)
        {
            this._next = next;
            this._localizer = localizer;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                await HandleValidationException(context, ex);
            }
            catch (Exception)
            {
                await HandleException(context);
            }
        }

        private async Task HandleValidationException(HttpContext context, ValidationException ex)
        {
            var errors = ex.Errors
                .Select(x => x.PropertyName + ": " + x.ErrorMessage)
                .ToList();

            var response = ResponseHandler.BadRequest<object>(_localizer[ValidationErrors.ValidationError]);
            response.Errors = errors;

            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(response);
        }

        private async Task HandleException(HttpContext context)
        {
            var response = ResponseHandler.BadRequest<object>(_localizer["SomethingWentWrong"]);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
