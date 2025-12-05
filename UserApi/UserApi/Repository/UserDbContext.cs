using Microsoft.EntityFrameworkCore;
using ServiceCommons;
using UserApi.Model;

namespace UserApi.Repository;

public class UserDbContext(DbContextOptions<UserDbContext> options) 
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<ProcessedEvent> ProcessedEvents => Set<ProcessedEvent>();
    public DbSet<UserProvisioningState> ProvisioningStates => Set<UserProvisioningState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);
    }
}