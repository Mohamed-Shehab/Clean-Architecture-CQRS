namespace CleanArchitecture.Application.Features.Courses.Queries.Get.Public.Modles
{
    public sealed class CourseSortingModel
    {
        public CourseOrderBy OrderBy { get; set; } = CourseOrderBy.Id;

        public bool IsDescending { get; set; } = false;
    }
}
