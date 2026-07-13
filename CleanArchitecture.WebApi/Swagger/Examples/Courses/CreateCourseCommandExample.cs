using CleanArchitecture.Application.Features.Courses.Commands.Create;
using Swashbuckle.AspNetCore.Filters;

namespace CleanArchitecture.WebApi.Swagger.Examples.Courses
{
    public class CreateCourseCommandExample : IExamplesProvider<CreateCourseCommand>
    {
        public CreateCourseCommand GetExamples()
        {
            return new CreateCourseCommand(
                NameEn: "ASP.NET Core Fundamentals",
                NameAr: "أساسيات ASP.NET Core",
                Description: "Learn the fundamentals of ASP.NET Core.",
                Capacity: 30,
                IsActive: true);
        }
    }
}
