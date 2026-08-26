using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Interfaces.Repositories
{
    public interface IUserSessionRepository : IRepository<UserSession>
    {
        Task<UserSession?> GetByRefreshTokenHashAsync(
            string refreshTokenHash,
            CancellationToken cancellationToken = default);
    }
}
