using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Persistence.Config;

public class SessionConfig : IEntityTypeConfiguration<Session>
{
  public void Configure(EntityTypeBuilder<Session> builder)
  {
    builder.ToTable("sessions");
    builder.HasKey(x => x.Id);

    builder.Property(x => x.UserId)
      .HasColumnName("user_id");
    
    builder.Property(x => x.RefreshTokenHash)
      .HasColumnName("refresh_token_hash")
      .IsRequired();

    builder.Property(x => x.CreatedAt)
      .HasColumnName("created_at")
      .IsRequired();

    builder.Property(x => x.ExpiresAt)
      .HasColumnName("expires_at")
      .IsRequired();

    builder.Property(x => x.Revoked)
      .HasDefaultValue(false)
      .HasColumnName("revoked")
      .IsRequired();

    builder.Property(x => x.Used)
      .HasDefaultValue(false)
      .HasColumnName("used")
      .IsRequired();

    builder.Property(x => x.ReplacedByTokenId)
      .HasColumnName("replaced_by_token_id");

    builder.Property(x => x.IpAddress)
      .HasColumnName("ip_address")
      .HasMaxLength(45)
      .IsRequired();

    builder.Property(x => x.UserAgent)
      .HasColumnName("user_agent")
      .HasMaxLength(1024)
      .IsRequired();

    builder.HasIndex(x => x.UserId);
    builder.HasIndex(x => x.RefreshTokenHash)
      .IsUnique();

    builder
      .HasOne(s => s.User)
      .WithMany()
      .HasForeignKey(s => s.UserId)
      .OnDelete(DeleteBehavior.NoAction);

  }
}