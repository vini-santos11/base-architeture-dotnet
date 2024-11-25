using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Mappings;

public class UserLoginMap : IEntityTypeConfiguration<UserLogin>
{
    public void Configure(EntityTypeBuilder<UserLogin> builder)
    {
        builder.ToTable(name: "UserLogin", tableBuilder => tableBuilder.IsTemporal());
        builder.HasKey(c => new { c.LoginProvider, c.ProviderKey});
    }
}