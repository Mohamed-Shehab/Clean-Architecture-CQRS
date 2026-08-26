using CleanArchitecture.Application.Common.Services.Authentication.Models;

namespace CleanArchitecture.Application.Common.Services.Authentication
{
    public interface IRefreshTokenService
    {
        RefreshTokenResult GenerateRefreshToken();

        string HashToken(string token);
    }
}