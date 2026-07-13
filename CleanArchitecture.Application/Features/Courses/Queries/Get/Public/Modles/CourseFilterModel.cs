namespace CleanArchitecture.Application.Features.Courses.Queries.Get.Public.Modles
{
    public sealed class CourseFilterModel
    {
        public string? Name { get; set; }

        public bool? HasAvailableSeats { get; set; }
    }
}
