namespace CleanArchitecture.Application.Features.Students.Queries.Get
{
    public class StudentDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;
    }
}
