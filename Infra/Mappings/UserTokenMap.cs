using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Mappings;

public class UserTokenMap : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.ToTable(name: "UserToken", tableBuilder => tableBuilder.IsTemporal());
        builder.HasKey(c => new { c.UserId, c.LoginProvider, c.Name });
    }
}