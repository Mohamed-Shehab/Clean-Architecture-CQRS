namespace CleanArchitecture.Application.Features.Courses.Queries.Get.Public
{
    public sealed class CourseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int AvailableSeats { get; set; }

        public bool IsFull { get; set; }

    }
}
