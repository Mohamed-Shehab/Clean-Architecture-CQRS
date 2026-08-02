using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CleanArchitecture.Infrastructure.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;



        public UnitOfWork(AppDbContext context)
        {
            this._context = context;
        }


        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }


        public async Task ExecuteInTransactionAsync(
            Func<Task> action,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            await ExecuteInTransactionAsync<object>(async () =>
            {
                await action();
                return null!;
            }, isolationLevel, cancellationToken);
        }


        public async Task<T> ExecuteInTransactionAsync<T>(
            Func<Task<T>> action,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);

            try
            {
                var result = await action();

                await transaction.CommitAsync(cancellationToken);

                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
