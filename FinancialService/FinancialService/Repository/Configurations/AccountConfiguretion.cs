using FinancialService.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinancialService.Repository.Configurations;

public class AccountConfiguretion : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Currency).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        
        builder.HasIndex(x => new { x.UserId, x.Currency }).IsUnique();
    }
}