using Microsoft.EntityFrameworkCore;
using ServiceCommons;
using SimpleAuth.Api.Model;

namespace SimpleAuth.Api.Repository;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<LoginAttempt> LoginAttempts => Set<LoginAttempt>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>(b =>
        {
            b.HasIndex(u => u.Email).IsUnique();
            b.Property(u => u.Version).IsRowVersion();
        });
        
        builder.Entity<RefreshToken>()
            .Property(r => r.Version)
            .IsRowVersion();
        
        builder.Entity<OutboxMessage>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Payload)
                .HasColumnType("jsonb");
            b.Property(x => x.CreatedAt)
                .HasDefaultValueSql("NOW()");
            b.HasIndex(x => x.ProcessedAt);
        });
        
        base.OnModelCreating(builder);
    }
}