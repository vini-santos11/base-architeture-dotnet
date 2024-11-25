using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Mappings;

public class UserMap : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User", tableBuilder => tableBuilder.IsTemporal());
        builder.HasKey(q => q.Id);

        builder.Property(q => q.Name).HasMaxLength(200);
        builder.Property(q => q.RegisterCode).HasMaxLength(6);
        builder.Property(q => q.RecoveryCode).HasMaxLength(6);
    }
}