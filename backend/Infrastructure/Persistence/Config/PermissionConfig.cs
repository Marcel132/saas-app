using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Persistence.Config;

public class PermissionConfig : IEntityTypeConfiguration<Permission>
{
  public void Configure(EntityTypeBuilder<Permission> builder)
  {
    builder.ToTable("permissions");
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
      .HasColumnName("id")
      .IsRequired();

    builder.Property(x => x.Resource)
      .HasColumnName("resource")
      .HasMaxLength(50)
      .IsRequired();

    builder.Property(x => x.Action)
      .HasColumnName("action")
      .HasMaxLength(50)
      .IsRequired();

    builder.Property(x => x.Code)
      .HasColumnName("code")
      .IsRequired();

    builder.Property(x => x.Description)
      .HasColumnName("description")
      .HasMaxLength(200)
      .IsRequired();

    builder.Property(x => x.IsActive)
      .HasColumnName("is_active")
      .HasDefaultValue(true)
      .IsRequired();

    builder.Property(x => x.CreatedAt)
      .HasColumnName("created_at")
      .IsRequired();

    builder.HasIndex(x => x.Code)
        .IsUnique();
  }
}