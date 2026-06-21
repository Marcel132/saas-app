using backend.Domain.EntitiesNew;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Persistence.Config;

public class UserPermissionConfig : IEntityTypeConfiguration<UserPermission>
{
  public void Configure(EntityTypeBuilder<UserPermission> builder)
  {
    builder.ToTable("user_permissions");
    builder.HasKey(x => new
    {
      x.UserId,
      x.PermissionId
    });

    builder.Property(x => x.UserId)
      .HasColumnName("user_id")
      .IsRequired();
    
    builder.Property(x => x.PermissionId)
      .HasColumnName("permission_id")
      .IsRequired();

    builder.Property(x => x.IsActive)
      .HasColumnName("is_active")
      .IsRequired();

    builder.Property(x => x.AssignedAt)
      .HasColumnName("assigned_at")
      .IsRequired();

    builder
      .HasOne<User>()
      .WithMany()
      .HasForeignKey(x => x.UserId)
      .OnDelete(DeleteBehavior.Cascade);

    builder
      .HasOne<Permission>()
      .WithMany()
      .HasForeignKey(x => x.PermissionId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}