using CleanArchitecture.Application.Common.Services.DatabaseException;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence.Services
{
    public class SqlServerDatabaseExceptionService : IDatabaseExceptionService
    {
        public bool IsUniqueConstraintViolation(DbUpdateException exception)
        {
            return exception.InnerException is SqlException sqlException
                   && (sqlException.Number == 2601 || sqlException.Number == 2627);
        }
    }
}
