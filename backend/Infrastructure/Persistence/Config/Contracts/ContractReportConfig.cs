using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Persistence.Config;

public class ContractReportConfig : IEntityTypeConfiguration<ContractReport>
{
  public void Configure(EntityTypeBuilder<ContractReport> builder)
  {
    builder.ToTable("contract_reports");
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
      .HasColumnName("id")
      .IsRequired();

    builder.Property(x => x.AssignmentId)
      .HasColumnName("assignment_id")
      .IsRequired();

    builder.Property(x => x.Status)
      .HasConversion<string>()
      .HasColumnName("status")
      .IsRequired();

    builder.Property(x => x.Feedback)
      .HasColumnName("feedback")
      .HasMaxLength(2000);

    builder.Property(x => x.CreatedAt)
      .HasColumnName("created_at")
      .IsRequired();

    builder.Property(x => x.ReviewedAt)
      .HasColumnName("reviewed_at");

    builder.Property(x => x.SubmittedAt)
      .HasColumnName("submitted_at");

    builder.Property(x => x.UpdatedAt)
      .HasColumnName("updated_at");

    
    builder.HasIndex(x => x.Status);

    builder
      .HasOne(x => x.ContractAssignment)
      .WithMany()
      .HasForeignKey(x => x.AssignmentId)
      .OnDelete(DeleteBehavior.NoAction);
  }
}