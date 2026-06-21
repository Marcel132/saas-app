using backend.Domain.EntitiesNew;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Persistence.Config;

public class ContractRequestConfig : IEntityTypeConfiguration<ContractRequest>
{
  public void Configure(EntityTypeBuilder<ContractRequest> builder)
  {
    builder.ToTable("contract_requests");
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
      .HasColumnName("id")
      .IsRequired();

    builder.Property(x => x.AssignmentId)
      .HasColumnName("assignment_id")
      .IsRequired();

    builder.Property(x => x.Title)
      .HasColumnName("title")
      .HasMaxLength(256)
      .IsRequired();

    builder.Property(x => x.Url)
      .HasColumnName("url")
      .HasMaxLength(256)
      .IsRequired();

    builder.Property(x => x.Scope)
      .HasColumnName("scope")
      .HasMaxLength(256)
      .IsRequired();

    builder.Property(x => x.Credentials)
      .HasColumnName("credentials")
      .IsRequired();

    builder.Property(x => x.Status)
      .HasColumnName("status")
      .HasConversion<string>()
      .IsRequired();

    builder.Property(x => x.Deadline)
      .HasColumnName("deadline")
      .IsRequired();

    builder.Property(x => x.IsActive)
      .HasColumnName("is_active")
      .IsRequired();

    builder.HasIndex(x => x.Status);

    builder
      .HasOne(x => x.ContractAssignment)
      .WithMany()
      .HasForeignKey(x => x.AssignmentId)
      .OnDelete(DeleteBehavior.NoAction);
      
  }
}