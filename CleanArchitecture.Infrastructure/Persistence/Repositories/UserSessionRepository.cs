using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Services.ClientInfo.Models;
using CleanArchitecture.Application.Common.Services.GeoLocation.Models;
using CleanArchitecture.Application.Features.Authentication.Queries.GetUserSessions;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories
{
    public class UserSessionRepository : Repository<UserSession>, IUserSessionRepository
    {
        public UserSessionRepository(AppDbContext context) : base(context)
        {
        }


        public async Task<UserSession?> GetByRefreshTokenHashAsync(string refreshTokenHash, 
                                                                   CancellationToken cancellationToken = default)
        {
            return await _context.UserSessions
                .FirstOrDefaultAsync(us => us.RefreshTokenHash == refreshTokenHash, cancellationToken);
        }


        public async Task<List<UserSessionDto>> GetActiveSessionsByUserIdAsync(int userId, 
                                                                               CancellationToken cancellationToken = default)
        {
            return await _context.UserSessions
                .Where(us => us.UserId == userId && !us.IsExpired && !us.IsRevoked)
                .OrderByDescending(us => us.LastUsedAt)
                .Select(us => new UserSessionDto
                {
                    UserSessionId = us.UserSessionId,

                    Device = new ClientDeviceInfo
                    {
                        DeviceType = us.DeviceType,
                        OperatingSystem = us.OperatingSystem,
                        Browser = us.Browser
                    },

                    Location = new GeoLocationInfo
                    {
                        Country = us.Country,
                        Region = us.Region,
                        City = us.City
                    },

                    LastUsedAt = us.LastUsedAt
                })
                .ToListAsync(cancellationToken);
        }


        public async Task<List<UserSession>> GetActiveSessionsEntitiesByUserIdAsync(int userId, 
                                                                                    CancellationToken cancellationToken = default)
        {
            return await _context.UserSessions
                .Where(us => us.UserId == userId && !us.IsExpired && !us.IsRevoked)
                .ToListAsync(cancellationToken);
        }
    }
}
