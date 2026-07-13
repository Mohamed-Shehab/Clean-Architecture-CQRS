namespace CleanArchitecture.Application.Features.Courses.Queries.GetById.Management
{
    public sealed class CourseManagementDetailsDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int Capacity { get; set; }

        public int EnrolledStudentsCount { get; set; }

        public int AvailableSeats => Capacity - EnrolledStudentsCount;

        public bool IsFull => Capacity <= EnrolledStudentsCount;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
