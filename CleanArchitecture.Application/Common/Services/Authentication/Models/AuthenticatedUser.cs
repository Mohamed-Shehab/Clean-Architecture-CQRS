namespace CleanArchitecture.Application.Common.Services.Authentication.Models
{
    public sealed class AuthenticatedUser
    {
        public int Id { get; init; }

        public string FirstName { get; init; } = default!;

        public string LastName { get; init; } = default!;

        public string Email { get; init; } = default!;

        public IReadOnlyCollection<string> Roles { get; init; } = [];

        public IReadOnlyCollection<string> Permissions { get; init; } = [];
    }
}
