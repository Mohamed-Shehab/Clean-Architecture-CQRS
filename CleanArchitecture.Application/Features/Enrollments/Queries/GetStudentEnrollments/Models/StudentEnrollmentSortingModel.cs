namespace CleanArchitecture.Application.Features.Enrollments.Queries.GetStudentEnrollments.Models
{
    public sealed class StudentEnrollmentSortingModel
    {
        public StudentEnrollmentOrderBy OrderBy { get; set; }
            = StudentEnrollmentOrderBy.EnrolledAt;

        public bool IsDescending { get; set; } = true;
    }
}
