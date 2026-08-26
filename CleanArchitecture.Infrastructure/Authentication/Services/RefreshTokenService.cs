using CleanArchitecture.Application.Common.Services.Authentication;
using CleanArchitecture.Application.Common.Services.Authentication.Models;
using CleanArchitecture.Infrastructure.Authentication.Configurations;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace CleanArchitecture.Infrastructure.Authentication.Services
{
    public sealed class RefreshTokenService : IRefreshTokenService
    {
        private readonly RefreshTokenOptions _refreshTokenOptions;


        public RefreshTokenService(IOptions<RefreshTokenOptions> refreshTokenOptions)
        {
            this._refreshTokenOptions = refreshTokenOptions.Value;
        }


        public RefreshTokenResult GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(32);

            var refreshToken = WebEncoders.Base64UrlEncode(randomBytes);


            return new RefreshTokenResult
            {
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenOptions.ExpirationInDays)
            };
        }


        public string HashToken(string token)
        {
            var tokenBytes = Encoding.UTF8.GetBytes(token);

            var hashBytes = SHA256.HashData(tokenBytes);

            return Convert.ToHexString(hashBytes);
        }
    }
}
