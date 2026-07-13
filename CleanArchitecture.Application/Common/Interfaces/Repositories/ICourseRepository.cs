using CleanArchitecture.Application.Common.Models.Querying;
using CleanArchitecture.Application.Features.Courses.Queries.Get.Management;
using CleanArchitecture.Application.Features.Courses.Queries.Get.Management.Models;
using CleanArchitecture.Application.Features.Courses.Queries.Get.Public;
using CleanArchitecture.Application.Features.Courses.Queries.Get.Public.Modles;
using CleanArchitecture.Application.Features.Courses.Queries.GetById.Management;
using CleanArchitecture.Application.Features.Courses.Queries.GetById.Public;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Interfaces.Repositories
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<int?> GetIdByNameAsync(
            string nameEn,
            string nameAr,
            CancellationToken cancellationToken);


        Task<int?> GetIdByNameAsync(
            int excludedCourseId,
            string nameEn, 
            string  nameAr, 
            CancellationToken cancellationToken);


        Task<(List<CourseManagementDto> Data, int TotalCount)> GetCoursesManagementAsync(
            CourseManagementFilterModel? filter,
            CourseManagementSortingModel? sorting,
            PaginationModel pagination,
            CancellationToken cancellationToken);


        Task<(List<CourseDto> Data, int TotalCount)> GetCoursesAsync(
            CourseFilterModel? filter,
            CourseSortingModel? sorting,
            PaginationModel pagination,
            CancellationToken cancellationToken);


        Task<CourseManagementDetailsDto?> GetCourseManagementByIdAsync(
            int courseId,
            CancellationToken cancellationToken);


        Task<CourseDetailsDto?> GetCourseByIdAsync(
            int courseId,
            CancellationToken cancellationToken);


    }
}
