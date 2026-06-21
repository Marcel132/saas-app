using backend.Domain.EntitiesNew;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Persistence.Config;
public class UserRoleConfig : IEntityTypeConfiguration<UserRole>
{
  public void Configure(EntityTypeBuilder<UserRole> builder)
  {
    builder.ToTable("user_roles");
    builder.HasKey(x => new
    {
      x.UserId,
      x.RoleId
    });

    builder.Property(x => x.UserId)
      .HasColumnName("user_id")
      .IsRequired();

    builder.Property(x => x.RoleId)
      .HasColumnName("role_id")
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
      .HasOne<Role>()
      .WithMany()
      .HasForeignKey(x => x.RoleId)
      .OnDelete(DeleteBehavior.Cascade);


  }
}