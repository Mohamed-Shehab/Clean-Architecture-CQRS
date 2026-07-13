namespace CleanArchitecture.Application.Common.Services.Identity.Models
{
    public sealed class CreateUserResult
    {
        public bool Succeeded { get; init; }

        public int UserId { get; init; }

        public List<string> Errors { get; init; } = [];
    }
}
