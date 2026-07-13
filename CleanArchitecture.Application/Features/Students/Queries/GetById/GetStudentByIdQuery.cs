using CleanArchitecture.Application.Common.Responses;
using MediatR;

namespace CleanArchitecture.Application.Features.Students.Queries.GetById
{
    public sealed record GetStudentByIdQuery(int Id) : IRequest<Response<StudentDetailsDto>>;
}
