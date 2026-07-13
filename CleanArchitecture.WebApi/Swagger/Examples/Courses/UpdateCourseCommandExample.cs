using CleanArchitecture.Application.Features.Courses.Commands.Update;
using Swashbuckle.AspNetCore.Filters;

namespace CleanArchitecture.WebApi.Swagger.Examples.Courses
{
    public class UpdateCourseCommandExample : IExamplesProvider<UpdateCourseCommand>
    {
        public UpdateCourseCommand GetExamples()
        {
            return new UpdateCourseCommand
            {
                NameEn = "Advanced ASP.NET Core",
                NameAr = "ASP.NET Core المتقدم",
                Description = "Updated course description.",
                Capacity = 40,
                IsActive = true
            };
        }
    }
}
