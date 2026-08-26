using CleanArchitecture.Application.Common.Services.ClientInfo;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Infrastructure.ClientInfo
{
    public sealed class ClientInfoProvider : IClientInfoProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ClientInfoProvider(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }


        public string? IpAddress =>
            _httpContextAccessor.HttpContext?
                .Connection
                .RemoteIpAddress?
                .ToString();


        public string? UserAgent =>
            _httpContextAccessor.HttpContext?
                .Request
                .Headers["User-Agent"]
                .FirstOrDefault();
    }
}
