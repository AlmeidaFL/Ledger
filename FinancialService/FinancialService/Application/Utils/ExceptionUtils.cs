using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FinancialService.Application.Utils;

public static class ExceptionUtils
{
    public static bool IsUniqueViolation(DbUpdateException ex)
    {
        return ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
    }
}