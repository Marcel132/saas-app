using backend.Domain.EntitiesNew;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Persistence.Config;

public class RolePermissionConfig : IEntityTypeConfiguration<RolePermission>
{
  public void Configure(EntityTypeBuilder<RolePermission> builder)
  {
    builder.ToTable("role_permissions");
    builder.HasKey(x => new
    {
      x.RoleId, 
      x.PermissionId
    });

    builder.Property(x => x.RoleId)
      .HasColumnName("role_id")
      .IsRequired();
    
    builder.Property(x => x.PermissionId)
      .HasColumnName("permission_id")
      .IsRequired();
      
    builder
      .HasOne<Role>()
      .WithMany()
      .HasForeignKey(x => x.RoleId)
      .OnDelete(DeleteBehavior.Cascade);

    builder
      .HasOne<Permission>()
      .WithMany()
      .HasForeignKey(x => x.PermissionId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}