namespace CleanArchitecture.Application.Features.Students.Queries.Get.Models
{
    public sealed class StudentFilterModel
    {
        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
