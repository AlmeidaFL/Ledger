using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserApi.Model;

namespace UserApi.Repository.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        
        builder.HasKey(x => x.Id);
        
        builder
            .Property(x => x.AccountNumber)
            .IsRequired()
            .HasMaxLength(20);
        
        builder
            .HasIndex(x => x.AccountNumber)
            .IsUnique();
        
        builder.Property(x => x.AccountType)
            .IsRequired()
            .HasMaxLength(20);
        
        builder
            .Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(x => x.RowVersion)
            .IsRowVersion();
        
        builder.HasOne(x => x.User)
            .WithOne(x => x.Account)
            .HasForeignKey<Account>(x => x.UserId);
        
        builder.HasIndex(x => x.UserId)
            .IsUnique();
    }
}