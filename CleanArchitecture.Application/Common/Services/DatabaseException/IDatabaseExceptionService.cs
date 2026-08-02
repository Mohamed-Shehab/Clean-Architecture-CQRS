using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Common.Services.DatabaseException
{
    public interface IDatabaseExceptionService
    {
        bool IsUniqueConstraintViolation(DbUpdateException exception);
    }
}
