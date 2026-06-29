using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Persistence.Config;

public class RoleConfig : IEntityTypeConfiguration<Role>
{
  public void Configure(EntityTypeBuilder<Role> builder)
  {
    builder.ToTable("roles");
    builder.HasKey(x => x.Id);

  
    builder.Property(x => x.Id)
      .HasColumnName("id")
      .IsRequired();

    builder.Property(x => x.Code)
      .HasColumnName("code")
      .IsRequired();

    builder.Property(x => x.Name)
      .HasColumnName("name")
      .IsRequired();

    builder.Property(x => x.IsActive)
      .HasColumnName("is_active")
      .HasDefaultValue(true)
      .IsRequired();

    builder.HasIndex(x => x.Code)
      .IsUnique();
  }
}