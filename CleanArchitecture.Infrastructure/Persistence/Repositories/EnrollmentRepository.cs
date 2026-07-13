using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Models.Querying;
using CleanArchitecture.Application.Common.Services.Localization;
using CleanArchitecture.Application.Features.Enrollments.Queries.GetCourseEnrollments;
using CleanArchitecture.Application.Features.Enrollments.Queries.GetCourseEnrollments.Models;
using CleanArchitecture.Application.Features.Enrollments.Queries.GetStudentEnrollments;
using CleanArchitecture.Application.Features.Enrollments.Queries.GetStudentEnrollments.Models;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories
{
    public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
    {
        private readonly ILocalizationService _localizationService;


        public EnrollmentRepository(AppDbContext context,
                                    ILocalizationService localizationService) : base(context)
        {
            this._localizationService = localizationService;
        }


        public async Task<bool> HasStudentsAsync(int courseId, 
                                                 CancellationToken cancellationToken)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.CourseId == courseId && e.Status != EnrollmentStatus.Active, cancellationToken);
        }


        public async Task<bool> HasAvailableSeatAsync(int courseId, 
                                                      int courseCapacity, 
                                                      CancellationToken cancellationToken)
        {
            var activeStudents = await _context.Enrollments
            .CountAsync(e => e.CourseId == courseId
                             && e.Status == EnrollmentStatus.Active, cancellationToken);

            return activeStudents < courseCapacity;
        }


        public async Task<Enrollment?> GetEnrollmentAsync(int studentId, 
                                                          int courseId, 
                                                          CancellationToken cancellationToken)
        {
            return await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId
                                          && e.CourseId == courseId, cancellationToken);
        }


        public async Task<(List<StudentEnrollmentDto> Data, int TotalCount)> GetStudentEnrollmentsAsync(int studentId,
                                                                                         StudentEnrollmentFilterModel? filter,
                                                                                         StudentEnrollmentSortingModel? sorting,
                                                                                         PaginationModel pagination,
                                                                                         CancellationToken cancellationToken)
        {
            var query = _context.Enrollments
                .Where(e => e.StudentId == studentId);


            // Filtering
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Name))
                {
                    var name = filter.Name.Trim();

                    query = query.Where(e =>
                        _localizationService.GetLocalized(e.Course.NameAr, e.Course.NameEn).Contains(name));
                }

                if (filter.Status.HasValue)
                {
                    query = query.Where(e =>
                        e.Status == filter.Status.Value);
                }
            }


            // Sorting
            var orderBy = sorting?.OrderBy ?? StudentEnrollmentOrderBy.EnrolledAt;
            var descending = sorting?.IsDescending ?? false;

            query = orderBy switch
            {
                StudentEnrollmentOrderBy.CourseName => _localizationService.IsArabic
                    ? descending
                        ? query.OrderByDescending(e => e.Course.NameAr)
                        : query.OrderBy(e => e.Course.NameAr)

                    : descending
                        ? query.OrderByDescending(e => e.Course.NameEn)
                        : query.OrderBy(e => e.Course.NameEn),


                StudentEnrollmentOrderBy.Status => descending
                        ? query.OrderByDescending(e => e.Status)
                        : query.OrderBy(e => e.Status),


                _ => descending
                        ? query.OrderByDescending(e => e.EnrolledAt)
                        : query.OrderBy(e => e.EnrolledAt)
            };


            // Total Count
            var totalCount = await query.CountAsync(cancellationToken);


            // Pagination + Projection
            var data = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(e => new StudentEnrollmentDto
                {
                    CourseId = e.CourseId,
                    Name = _localizationService.GetLocalized(e.Course.NameAr, e.Course.NameEn),
                    Status = e.Status,
                    EnrolledAt = e.EnrolledAt,
                    CompletedAt = e.CompletedAt,
                    DroppedAt = e.DroppedAt
                })
                .ToListAsync(cancellationToken);
            

            return (data, totalCount);
        }

        public async Task<(List<CourseEnrollmentDto> Data, int TotalCount)> GetCourseEnrollmentsAsync(int courseId, 
                                                                                                CourseEnrollmentFilterModel? filter, 
                                                                                                CourseEnrollmentSortingModel? sorting, 
                                                                                                PaginationModel pagination, 
                                                                                                CancellationToken cancellationToken)
        {
            var query = _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Select(e => new
                {
                    e.StudentId,
                    e.Student.UserId,
                    e.Status,
                    e.EnrolledAt,
                    e.CompletedAt,
                    e.DroppedAt
                })
                .Join(_context.Users,
                    x => x.UserId,
                    user => user.Id,
                    (x, user) => new
                    {
                        x.StudentId,
                        FullName = user.FirstName + " " + user.LastName,
                        user.Email,
                        user.PhoneNumber,
                        x.Status,
                        x.EnrolledAt,
                        x.CompletedAt,
                        x.DroppedAt
                    });


            // Filtering
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.StudentName))
                {
                    var name = filter.StudentName.Trim().ToLower();
                    query = query.Where(x => x.FullName.ToLower().Contains(name));
                }

                if (!string.IsNullOrWhiteSpace(filter.Email))
                {
                    query = query.Where(x => x.Email == filter.Email.Trim());
                }

                if (!string.IsNullOrWhiteSpace(filter.PhoneNumber))
                {
                    query = query.Where(x => x.PhoneNumber == filter.PhoneNumber.Trim());
                }

                if (filter.Status != null)
                {
                    query = query.Where(x => x.Status == filter.Status);
                }
            }


            // Sorting
            var orderBy = sorting?.OrderBy ?? CourseEnrollmentOrderBy.EnrolledAt;
            var descending = sorting?.IsDescending ?? false;

            query = orderBy switch
            {
                CourseEnrollmentOrderBy.StudentName => descending
                    ? query.OrderByDescending(x => x.FullName)
                    : query.OrderBy(x => x.FullName),


                CourseEnrollmentOrderBy.Status => descending
                    ? query.OrderByDescending(x => x.Status)
                    : query.OrderBy(x => x.Status),


                _ => descending
                    ? query.OrderByDescending(x => x.EnrolledAt)
                    : query.OrderBy(x => x.EnrolledAt)
            };


            // Total Count
            var totalCount = await query.CountAsync(cancellationToken);


            // Pagination + Projection
            var students = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(x => new CourseEnrollmentDto
                {
                    StudentId = x.StudentId,
                    StudentName = x.FullName,
                    Status = x.Status,
                    EnrolledAt = x.EnrolledAt,
                    CompletedAt = x.CompletedAt,
                    DroppedAt = x.DroppedAt
                })
                .ToListAsync(cancellationToken);


            return (students, totalCount);
        }


    }
}
