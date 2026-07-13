using CleanArchitecture.Application.Common.Models.Querying;
using CleanArchitecture.Application.Features.Students.Queries.Get;
using CleanArchitecture.Application.Features.Students.Queries.Get.Models;
using CleanArchitecture.Application.Features.Students.Queries.GetById;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Interfaces.Repositories
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<(List<StudentDto> Data, int TotalCount)> GetStudentsAsync(
            StudentFilterModel? filter,
            StudentSortingModel? sorting,
            PaginationModel pagination,
            CancellationToken cancellationToken);


        Task<StudentDetailsDto?> GetStudentDetailsAsync(
            int studentId,
            CancellationToken cancellationToken);

    }
}
