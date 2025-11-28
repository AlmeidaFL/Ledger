using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserApi.Model;

namespace UserApi.Repository.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder
            .Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.HasIndex(x => x.Email).IsUnique();
        
        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(x => x.RowVersion)
            .IsRowVersion();
        
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("NOW()");
    }
}