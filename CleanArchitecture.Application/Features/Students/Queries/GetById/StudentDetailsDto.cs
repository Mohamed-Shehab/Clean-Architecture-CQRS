namespace CleanArchitecture.Application.Features.Students.Queries.GetById
{
    public sealed class StudentDetailsDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public DateOnly DateOfBirth { get; set; }

        public string? Address { get; set; }
    }
}
