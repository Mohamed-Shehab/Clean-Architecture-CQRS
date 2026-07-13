namespace CleanArchitecture.Application.Features.Courses.Queries.Get.Management.Models
{
    public sealed class CourseManagementSortingModel
    {
        public CourseManagementOrderBy OrderBy { get; set; } = CourseManagementOrderBy.Id;

        public bool IsDescending { get; set; } = false;
    }
}
