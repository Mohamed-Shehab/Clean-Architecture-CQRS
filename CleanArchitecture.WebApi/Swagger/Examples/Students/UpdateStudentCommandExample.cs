using CleanArchitecture.Application.Features.Students.Commands.Update;
using Swashbuckle.AspNetCore.Filters;

namespace CleanArchitecture.WebApi.Swagger.Examples.Students
{
    public class UpdateStudentCommandExample : IExamplesProvider<UpdateStudentCommand>
    {
        public UpdateStudentCommand GetExamples()
        {
            return new UpdateStudentCommand
            {
                FirstName = "Mohamed",
                LastName = "Shehab",
                PhoneNumber = "01013648744",
                DateOfBirth = new DateOnly(2005, 5, 12),
                Address = "Menoufia"
            };
        }
    }
}
