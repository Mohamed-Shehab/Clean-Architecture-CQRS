namespace CleanArchitecture.Application.Features.Courses.Queries.GetById.Public
{
    public sealed class CourseDetailsDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsFull => AvailableSeats <= 0;

        public int AvailableSeats { get; set; }
    }
}
