namespace CleanArchitecture.Application.Features.Courses.Queries.Get.Management
{
    public sealed class CourseManagementDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int EnrolledStudentsCount { get; set; }

        public int AvailableSeats { get; set; }

        public bool IsActive { get; set; }
    }
}
