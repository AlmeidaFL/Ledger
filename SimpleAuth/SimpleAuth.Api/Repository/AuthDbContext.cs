using Microsoft.EntityFrameworkCore;

namespace SimpleAuth.Api.Data;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<LoginAttempt> LoginAttempts => Set<LoginAttempt>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        
        builder.Entity<User>()
            .Property(u => u.RowVersion)
            .IsRowVersion();
        
        builder.Entity<RefreshToken>()
            .Property(r => r.RowVersion)
            .IsRowVersion();
    }
}