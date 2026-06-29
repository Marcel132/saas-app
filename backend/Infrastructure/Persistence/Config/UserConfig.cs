using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Persistence.Config;

public class UserConfig : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.ToTable("users");
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
      .HasColumnName("id")
      .IsRequired();

    builder.Property(x => x.Email)
      .HasMaxLength(256)
      .HasColumnName("email")
      .IsRequired();

    builder.Property(x => x.PasswordHash)
      .HasColumnName("password_hash")
      .IsRequired();

    builder.Property(x => x.IsActive)
      .HasDefaultValue(true)
      .HasColumnName("is_active")
      .IsRequired();

    builder.Property(x => x.RoleType)
      .HasConversion<string>()
      .HasColumnName("role")
      .IsRequired();

    builder.Property(x => x.CreatedAt)
      .HasColumnName("created_at")
      .IsRequired();

    builder.Property(x => x.FailedLoginAttempts)
      .HasDefaultValue(0)
      .HasColumnName("failed_login_attempts")
      .IsRequired();

    builder.Property(x => x.LoginBlockedUntil)
      .HasColumnName("login_blocked_until");

    builder.HasIndex(x => x.Email)
      .IsUnique();

   
    builder.Navigation(x => x.UserRoles)
      .UsePropertyAccessMode(PropertyAccessMode.Field);

    builder.Navigation(x => x.UserPermissions)
      .UsePropertyAccessMode(PropertyAccessMode.Field);
  }
}