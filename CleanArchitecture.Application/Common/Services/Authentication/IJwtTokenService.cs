using CleanArchitecture.Application.Common.Services.Authentication.Models;

namespace CleanArchitecture.Application.Common.Services.Authentication
{
    public interface IJwtTokenService
    {
        AccessTokenResult GenerateAccessToken(AuthenticatedUser user);


        string GeneratePurposeToken(
            string userId,
            string purpose,
            TimeSpan lifetime);
    }
}
