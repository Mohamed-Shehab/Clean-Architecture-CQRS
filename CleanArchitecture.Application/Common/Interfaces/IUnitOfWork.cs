using System.Data;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken);


        Task ExecuteInTransactionAsync(
            Func<Task> action,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default);


        Task<T> ExecuteInTransactionAsync<T>(
            Func<Task<T>> action,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default);
    }
}
