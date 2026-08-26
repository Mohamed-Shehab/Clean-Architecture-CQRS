using CleanArchitecture.Application.Common.Interfaces.Repositories;
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
    }
}
