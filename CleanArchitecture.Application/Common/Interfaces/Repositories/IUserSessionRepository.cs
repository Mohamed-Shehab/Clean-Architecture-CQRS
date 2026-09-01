using CleanArchitecture.Application.Features.Authentication.Queries.GetUserSessions;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Interfaces.Repositories
{
    public interface IUserSessionRepository : IRepository<UserSession>
    {
        Task<UserSession?> GetByRefreshTokenHashAsync(
            string refreshTokenHash,
            CancellationToken cancellationToken = default);


        Task<List<UserSessionDto>> GetActiveSessionsByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);


        Task<List<UserSession>> GetActiveSessionsEntitiesByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);
    }
}
