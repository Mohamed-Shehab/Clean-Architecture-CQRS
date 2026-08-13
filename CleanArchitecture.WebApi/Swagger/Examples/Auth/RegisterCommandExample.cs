using CleanArchitecture.Application.Features.Authentication.Commands.Register;
using Swashbuckle.AspNetCore.Filters;

namespace CleanArchitecture.WebApi.Swagger.Examples.Auth
{
    public class RegisterCommandExample : IExamplesProvider<RegisterCommand>
    {
        public RegisterCommand GetExamples()
        {
            return new RegisterCommand(
                "Mohamed",
                "Shehab",
                "mohamed@gmail.com",
                "01013648744",
                "P@ssw0rd",
                new DateOnly(2005, 5, 12),
                "Menoufia"
            );
        }
    }
}
