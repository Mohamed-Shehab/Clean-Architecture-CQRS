using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Models.Querying;
using CleanArchitecture.Application.Common.Services.Localization;
using CleanArchitecture.Application.Features.Courses.Queries.Get.Management;
using CleanArchitecture.Application.Features.Courses.Queries.Get.Management.Models;
using CleanArchitecture.Application.Features.Courses.Queries.Get.Public;
using CleanArchitecture.Application.Features.Courses.Queries.Get.Public.Modles;
using CleanArchitecture.Application.Features.Courses.Queries.GetById.Management;
using CleanArchitecture.Application.Features.Courses.Queries.GetById.Public;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        private readonly ILocalizationService _localizationService;


        public CourseRepository(AppDbContext context,
                                ILocalizationService localizationService) : base(context)
        {
            this._localizationService = localizationService;
        }


        public async Task<int?> GetIdByNameAsync(string nameEn,
                                                 string nameAr,
                                                 CancellationToken cancellationToken)
        {
            return await _context.Courses
                .Where(c => c.NameEn == nameEn || c.NameAr == nameAr)
                .Select(c => (int?)c.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }
        

        public async Task<int?> GetIdByNameAsync(int excludedCourseId,
                                                 string nameEn,
                                                 string nameAr,
                                                 CancellationToken cancellationToken)
        {
            return await _context.Courses
                .Where(c => c.Id != excludedCourseId
                            && (c.NameEn == nameEn || c.NameAr == nameAr))
                .Select(c => (int?)c.Id)
                .FirstOrDefaultAsync(cancellationToken);
                
        }


        public async Task<(List<CourseManagementDto> Data, int TotalCount)> GetCoursesManagementAsync(CourseManagementFilterModel? filter,
                                                                                  CourseManagementSortingModel? sorting,
                                                                                  PaginationModel pagination,
                                                                                  CancellationToken cancellationToken)
        {
            // Base query
            var query = _context.Courses.AsQueryable();


            // Filtration
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Name))
                {
                    var name = filter.Name.Trim();
                    query = query.Where(c => c.NameEn.Contains(name) || c.NameAr.Contains(name));
                }

                if (filter.IsActive != null)
                {
                    query = query.Where(c => c.IsActive == filter.IsActive);
                }
            }


            // Sorting
            var orderBy = sorting?.OrderBy ?? CourseManagementOrderBy.Id;
            var descending = sorting?.IsDescending ?? false;

            query = orderBy switch
            {
                CourseManagementOrderBy.Name => _localizationService.IsArabic
                    ? descending 
                        ? query.OrderByDescending(c => c.NameAr) 
                        : query.OrderBy(c => c.NameAr)

                    : descending 
                        ? query.OrderByDescending(c => c.NameEn) 
                        : query.OrderBy(c => c.NameEn),


                CourseManagementOrderBy.Capacity => descending 
                    ? query.OrderByDescending(c => c.Capacity) 
                    : query.OrderBy(c => c.Capacity),


                CourseManagementOrderBy.EnrolledStudents => descending 
                    ? query.OrderByDescending(c => c.Enrollments.Count(e => e.Status == EnrollmentStatus.Active)) 
                    : query.OrderBy(c => c.Enrollments.Count(e => e.Status == EnrollmentStatus.Active)),

                CourseManagementOrderBy.IsActive => descending 
                    ? query.OrderByDescending(c => c.IsActive) 
                    : query.OrderBy(c => c.IsActive),

                _ => descending 
                    ? query.OrderByDescending(c => c.Id) 
                    : query.OrderBy(c => c.Id)
            };


            // Total count Before pagination
            var totalCount = await query.CountAsync(cancellationToken);


            // Pagination + Projection
            var courses = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(c => new
                {
                    Course = c,
                    EnrolledStudentsCount = c.Enrollments.Count(e => e.Status == EnrollmentStatus.Active)
                })
                .Select(x => new CourseManagementDto
                {
                    Id = x.Course.Id,

                    Name = _localizationService.GetLocalized(x.Course.NameAr, x.Course.NameEn),

                    EnrolledStudentsCount = x.EnrolledStudentsCount,

                    AvailableSeats = x.Course.Capacity - x.EnrolledStudentsCount,

                    IsActive = x.Course.IsActive
                })
                .ToListAsync(cancellationToken);


            return (courses, totalCount);
        }


        public async Task<(List<CourseDto> Data, int TotalCount)> GetCoursesAsync(CourseFilterModel? filter,
                                                                                  CourseSortingModel? sorting,
                                                                                  PaginationModel pagination,
                                                                                  CancellationToken cancellationToken)
        {
            // Base query
            var query = _context.Courses
                .Where(c => c.IsActive);


            // Filtration
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Name))
                {
                    var name = filter.Name.Trim();
                    query = query.Where(c => c.NameEn.Contains(name) || c.NameAr.Contains(name));
                }
                
                if (filter.HasAvailableSeats.HasValue)
                {
                    if (filter.HasAvailableSeats.Value)
                        query = query.Where(c => c.Capacity > c.Enrollments.Count(e => e.Status == EnrollmentStatus.Active));

                    else
                        query = query.Where(c => c.Capacity <= c.Enrollments.Count(e => e.Status == EnrollmentStatus.Active));

                }
            }


            // Sorting
            var orderBy = sorting?.OrderBy ?? CourseOrderBy.Id;
            var descending = sorting?.IsDescending ?? false;

            query = orderBy switch
            {
                CourseOrderBy.Name => _localizationService.IsArabic
                    ? descending
                        ? query.OrderByDescending(c => c.NameAr)
                        : query.OrderBy(c => c.NameAr)

                    : descending
                        ? query.OrderByDescending(c => c.NameEn)
                        : query.OrderBy(c => c.NameEn),


                CourseOrderBy.AvailableSeats => descending
                    ? query.OrderByDescending(c => c.Capacity - c.Enrollments.Count(e => e.Status == EnrollmentStatus.Active))
                    : query.OrderBy(c => c.Capacity - c.Enrollments.Count(e => e.Status == EnrollmentStatus.Active)),


                _ => descending
                    ? query.OrderByDescending(c => c.Id)
                    : query.OrderBy(c => c.Id)
            };


            // Total count Before pagination
            var totalCount = await query.CountAsync(cancellationToken);


            // Pagination + Projection
            var courses = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(c => new
                {
                    Course = c,
                    EnrolledStudentsCount = c.Enrollments.Count(e => e.Status == EnrollmentStatus.Active)
                })
                .Select(x => new CourseDto
                {
                    Id = x.Course.Id,

                    Name = _localizationService.GetLocalized(x.Course.NameAr, x.Course.NameEn),

                    AvailableSeats = x.Course.Capacity - x.EnrolledStudentsCount,

                    IsFull =  x.Course.Capacity <= x.EnrolledStudentsCount
                })
                .ToListAsync(cancellationToken);


            return (courses, totalCount);
        }


        public async Task<CourseManagementDetailsDto?> GetCourseManagementByIdAsync(int courseId, 
                                                                                    CancellationToken cancellationToken)
        {
            return await _context.Courses
                .Where(c => c.Id == courseId)
                .Select(c => new CourseManagementDetailsDto
                {
                    Id = c.Id,
                    Name = _localizationService.GetLocalized(c.NameAr, c.NameEn),
                    Description = c.Description,
                    Capacity = c.Capacity,
                    EnrolledStudentsCount = c.Enrollments.Count(e => e.Status == EnrollmentStatus.Active),
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);
        }


        public async Task<CourseDetailsDto?> GetCourseByIdAsync(int courseId,
                                                                CancellationToken cancellationToken)
        {
            return await _context.Courses
                .Where(c => c.Id == courseId && c.IsActive)
                .Select(c => new CourseDetailsDto
                {
                    Id = c.Id,

                    Name = _localizationService.GetLocalized(c.NameAr, c.NameEn),

                    Description = c.Description,

                    AvailableSeats = c.Capacity - c.Enrollments.Count(e => e.Status == EnrollmentStatus.Active)
                })
                .FirstOrDefaultAsync(cancellationToken);
        }


    }
}
