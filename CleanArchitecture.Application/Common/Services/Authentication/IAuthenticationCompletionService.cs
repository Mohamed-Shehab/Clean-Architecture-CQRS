using CleanArchitecture.Application.Common.Services.Authentication.Models;
using CleanArchitecture.Application.Features.Authentication.Models;

namespace CleanArchitecture.Application.Common.Services.Authentication
{
    public interface IAuthenticationCompletionService
    {
        Task<TokenResponse> CompleteAuthenticationAsync(
            AuthenticatedUser user,
            CancellationToken cancellationToken);
    }
}
