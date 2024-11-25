using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Mappings;

public class UserRoleMap : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable(name: "UserRole", tableBuilder => tableBuilder.IsTemporal());
        builder.HasKey(c => new { c.UserId, c.RoleId});
    }
}