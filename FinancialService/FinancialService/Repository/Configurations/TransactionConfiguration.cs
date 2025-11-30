using FinancialService.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinancialService.Repository.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder
            .Property(x => x.IdempotencyKey)
            .IsRequired()
            .ValueGeneratedNever();

        builder.HasIndex(x => x.IdempotencyKey).IsUnique();

        builder.Property(x => x.Type)
            .IsRequired();
        
        builder.Property(x => x.Metadata)
            .HasColumnType("jsonb")
            .HasDefaultValue("{}");
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasMany(x => x.JournalEntries)
            .WithOne(x => x.Transaction)
            .HasForeignKey(x => x.TransactionId);
    }
}