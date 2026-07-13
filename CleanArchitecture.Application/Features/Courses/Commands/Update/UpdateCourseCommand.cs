using CleanArchitecture.Application.Common.Responses;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArchitecture.Application.Features.Courses.Commands.Update
{
    public sealed record UpdateCourseCommand : IRequest<Response<int>>
    {
        [JsonIgnore]
        public int Id { get; init; }

        public string NameEn { get; init; } = null!;

        public string NameAr { get; init; } = null!;

        public string? Description { get; init; }

        public int Capacity { get; init; }

        public bool IsActive { get; init; }
    }
}
