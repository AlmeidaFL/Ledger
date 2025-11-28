using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserApi.Model;

namespace UserApi.Repository.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Payload)
            .HasColumnType("jsonb");

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("NOW()");
        
        builder.HasIndex(x => x.ProcessedAt);
    }
}