namespace CleanArchitecture.Application.Features.Courses.Queries.Get.Management.Models
{
    public sealed class CourseManagementFilterModel
    {
        public string? Name { get; set; }

        public bool? IsActive { get; set; }
    }
}
