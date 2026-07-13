namespace CleanArchitecture.Application.Features.Enrollments.Queries.GetCourseEnrollments.Models
{
    public sealed class CourseEnrollmentSortingModel
    {
        public CourseEnrollmentOrderBy OrderBy { get; set; }
            = CourseEnrollmentOrderBy.EnrolledAt;

        public bool IsDescending { get; set; } = true;
    }
}
