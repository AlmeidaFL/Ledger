using FinancialService.Model;
using FinancialService.Repository;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ServiceCommons;

namespace FinancialService.Application.Services;

public interface IBalanceService
{
    public Task<Result<GetBalanceResponse>> GetBalance(string userEmail, CancellationToken cancellationToken = default);
}

public class BalanceService(FinancialDbContext db) : IBalanceService
{
    public async Task<Result<GetBalanceResponse>> GetBalance(string userEmail, CancellationToken cancellationToken = default)
    {
        var account = await db.Accounts.Include(a => a.User)
            .Where(a => a.User.Email == userEmail)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (account == null)
        {
            return Result<GetBalanceResponse>.Failure("Account not found");
        }
        
        var balance = await CalculateBalanceAsync(account.Id, cancellationToken);

        var response = new GetBalanceResponse
        {
            UserEmail = userEmail,
            BalanceInCents = balance,
            Currency = account.Currency,
        };
        return Result<GetBalanceResponse>.Success(response);
    }
    
    private async Task<long> CalculateBalanceAsync(Guid accountId, CancellationToken ct)
    {
        return await db.JournalEntries
            .Where(e => e.AccountId == accountId)
            .SumAsync(
                e => e.Type == EntryType.Credit ? e.Amount : -e.Amount,
                ct
            );
    }
}