using CleanArchitecture.Application.Common.Services.CurrentUser;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CleanArchitecture.Infrastructure.CurrentUser
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;


        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }


        public bool IsAuthenticated => 
            _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;


        public int UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?
                    .User
                    .FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim is null)
                    throw new InvalidOperationException(
                        "The current user ID claim is missing.");


                if (!int.TryParse(userIdClaim.Value, out var userId))
                    throw new InvalidOperationException(
                        "The current user ID claim is invalid.");


                return userId;
            }
        }


        public string Email
        {
            get
            {
                var emailClaim = _httpContextAccessor.HttpContext?
                    .User
                    .FindFirst(ClaimTypes.Email);

                if (emailClaim is null)
                    throw new InvalidOperationException(
                        "The current user email claim is missing.");


                return emailClaim.Value;
            }
        }
    }
}
