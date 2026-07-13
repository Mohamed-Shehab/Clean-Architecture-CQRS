using CleanArchitecture.Application.Features.Students.Commands.Create;
using Swashbuckle.AspNetCore.Filters;

namespace CleanArchitecture.WebApi.Swagger.Examples.Students
{
    public class CreateStudentCommandExample : IExamplesProvider<CreateStudentCommand>
    {
        public CreateStudentCommand GetExamples()
        {
            return new CreateStudentCommand(
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
