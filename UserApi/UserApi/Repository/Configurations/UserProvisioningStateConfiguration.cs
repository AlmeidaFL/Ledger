using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserApi.Model;

namespace UserApi.Repository.Configurations;

public class UserProvisioningStateConfiguration : IEntityTypeConfiguration<UserProvisioningState>
{
    public void Configure(EntityTypeBuilder<UserProvisioningState> builder)
    {
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}