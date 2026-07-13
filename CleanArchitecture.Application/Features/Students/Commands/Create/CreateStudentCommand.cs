using CleanArchitecture.Application.Common.Responses;
using MediatR;

namespace CleanArchitecture.Application.Features.Students.Commands.Create
{
    public sealed record CreateStudentCommand(
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        string Password,
        DateOnly DateOfBirth,
        string? Address
    ) : IRequest<Response<int>>;
}
