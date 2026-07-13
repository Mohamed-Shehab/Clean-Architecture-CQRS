namespace CleanArchitecture.Application.Features.Students.Queries.Get.Models
{
    public sealed class StudentSortingModel
    {
        public StudentOrderBy OrderBy { get; set; } = StudentOrderBy.Id;

        public bool IsDescending { get; set; }
    }
}
