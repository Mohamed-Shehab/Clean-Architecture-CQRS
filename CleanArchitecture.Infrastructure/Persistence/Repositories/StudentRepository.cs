using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Models.Querying;
using CleanArchitecture.Application.Features.Students.Queries.Get;
using CleanArchitecture.Application.Features.Students.Queries.Get.Models;
using CleanArchitecture.Application.Features.Students.Queries.GetById;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(AppDbContext context) : base(context)
        {
        }


        public async Task<(List<StudentDto> Data, int TotalCount)> GetStudentsAsync(StudentFilterModel? filter,
                                                                                    StudentSortingModel? sorting,
                                                                                    PaginationModel pagination,
                                                                                    CancellationToken cancellationToken)
        {
            var query = _context.Students
                .Join(
                    _context.Users,
                    student => student.UserId,
                    user => user.Id,
                    (student, user) => new
                    {
                        StudentId = student.Id,
                        FullName = user.FirstName + " " + user.LastName,
                        user.Email,
                        user.PhoneNumber,
                        student.DateOfBirth
                    });


            // Filtering
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.FullName))
                {
                    var fullName = filter.FullName.Trim();

                    query = query.Where(x =>
                        x.FullName.Contains(fullName));
                }

                if (!string.IsNullOrWhiteSpace(filter.Email))
                {
                    var email = filter.Email.Trim();

                    query = query.Where(x =>
                        x.Email!.Contains(email));
                }

                if (!string.IsNullOrWhiteSpace(filter.PhoneNumber))
                {
                    var phoneNumber = filter.PhoneNumber.Trim();

                    query = query.Where(x =>
                        x.PhoneNumber!.Contains(phoneNumber));
                }
            }


            // Sorting
            var orderBy = sorting?.OrderBy ?? StudentOrderBy.Id;
            var descending = sorting?.IsDescending ?? false;

            query = orderBy switch
            {
                StudentOrderBy.FullName => descending
                    ? query.OrderByDescending(x => x.FullName)
                    : query.OrderBy(x => x.FullName),

                StudentOrderBy.DateOfBirth => descending
                    ? query.OrderByDescending(x => x.DateOfBirth)
                    : query.OrderBy(x => x.DateOfBirth),

                _ => descending
                    ? query.OrderByDescending(x => x.StudentId)
                    : query.OrderBy(x => x.StudentId)
            };


            // Total Count
            var totalCount = await query.CountAsync(cancellationToken);


            // Pagination + Projection
            var students = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select( x => new StudentDto
                {
                    Id = x.StudentId,
                    FullName = x.FullName,
                    Email = x.Email!,
                    PhoneNumber = x.PhoneNumber!
                })
                .ToListAsync(cancellationToken);


            return (students, totalCount);
        }


        public async Task<StudentDetailsDto?> GetStudentDetailsAsync(int studentId,
                                                                     CancellationToken cancellationToken)
        {
            return await _context.Students
                .Where(s => s.Id == studentId)
                .Join(
                    _context.Users,
                    student => student.UserId,
                    user => user.Id,
                    (student, user) => new StudentDetailsDto
                    {
                        Id = student.Id,
                        FullName = user.FirstName + " " + user.LastName,
                        Email = user.Email!,
                        PhoneNumber = user.PhoneNumber!,
                        DateOfBirth = student.DateOfBirth,
                        Address = student.Address
                    })
                .FirstOrDefaultAsync(cancellationToken);
        }


    }
}
