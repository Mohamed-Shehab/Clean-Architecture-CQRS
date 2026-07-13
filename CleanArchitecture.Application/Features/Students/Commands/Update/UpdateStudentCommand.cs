using CleanArchitecture.Application.Common.Responses;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArchitecture.Application.Features.Students.Commands.Update
{
    public sealed record UpdateStudentCommand : IRequest<Response<object>>
    {
        [JsonIgnore]
        public int Id { get; init; }

        public string FirstName { get; init; } = null!;

        public string LastName { get; init; } = null!;

        public string PhoneNumber { get; init; } = null!;

        public DateOnly DateOfBirth { get; init; }

        public string? Address { get; init; }
    }
}
