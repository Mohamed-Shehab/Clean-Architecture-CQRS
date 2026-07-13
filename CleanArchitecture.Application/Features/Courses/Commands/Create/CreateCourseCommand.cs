using CleanArchitecture.Application.Common.Responses;
using MediatR;

namespace CleanArchitecture.Application.Features.Courses.Commands.Create
{
    public sealed record CreateCourseCommand(string NameEn,
                                             string NameAr,
                                             string? Description,
                                             int Capacity,
                                             bool IsActive = true) : IRequest<Response<int>>;
}
