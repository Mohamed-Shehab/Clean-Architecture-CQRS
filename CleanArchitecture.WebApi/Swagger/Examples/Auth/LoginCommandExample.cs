using CleanArchitecture.Application.Features.Authentication.Commands.Login;
using Swashbuckle.AspNetCore.Filters;

namespace CleanArchitecture.WebApi.Swagger.Examples.Auth
{
    public class LoginCommandExample : IExamplesProvider<LoginCommand>
    {
        public LoginCommand GetExamples()
        {
            return new LoginCommand(
                "mohamedshehab@gmail.com",
                "P@ssw0rd"
            );
        }   
    }
}
