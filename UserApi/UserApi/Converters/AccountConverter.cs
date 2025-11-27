using UserApi.Dtos;
using UserApi.Model;

namespace UserApi.Converters;

public class AccountConverter
{
    public static AccountResponse ToResponse(Account account)
        => new()
        {
            Id = account.Id,
            UserId = account.UserId,
            AccountNumber = account.AccountNumber,
            AccountType = account.AccountType,
            Status = account.Status,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt
        }; 
}