using FinancialService.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinancialService.Repository.Configurations;

public class AccountLockConfiguration : IEntityTypeConfiguration<AccountLock>
{
    public void Configure(EntityTypeBuilder<AccountLock> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever();
    }
}