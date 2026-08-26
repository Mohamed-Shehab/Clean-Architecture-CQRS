using CleanArchitecture.Application.Common.Services.Authentication;
using CleanArchitecture.Application.Common.Services.Authentication.Models;
using CleanArchitecture.Infrastructure.Authentication.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CleanArchitecture.Infrastructure.Authentication.Services
{
    public sealed class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions _jwtOptions;


        public JwtTokenService(IOptions<JwtOptions> jwtOptions)
        {
            this._jwtOptions = jwtOptions.Value;
        }


        public AccessTokenResult GenerateAccessToken(AuthenticatedUser user)
        {
            // Generate claims for the user
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.GivenName, user.FirstName),
                new(ClaimTypes.Surname, user.LastName)
            };

            
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }


            foreach (var permission in user.Permissions)
            {
                claims.Add(new Claim("permission", permission));
            }


            // Create the signing key and credentials
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var  expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes);


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = credentials,
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var accessToken = tokenHandler.WriteToken(token);


            return new AccessTokenResult
            {
                AccessToken = accessToken,
                ExpiresAt = expires
            };
        }
    }
}
