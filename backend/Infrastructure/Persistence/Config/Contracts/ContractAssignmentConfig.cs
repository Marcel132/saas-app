using backend.Domain.EntitiesNew;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Persistence.Config;

public class ContractAssignmentConfig : IEntityTypeConfiguration<ContractAssignment>
{
  public void Configure(EntityTypeBuilder<ContractAssignment> builder)
  {
    builder.ToTable("contract_assignments");
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
      .HasColumnName("id")
      .IsRequired();

    builder.Property(x => x.ContractId)
      .HasColumnName("contract_id")
      .IsRequired();

    builder.Property(x => x.PentesterId)
      .HasColumnName("pentester_id")
      .IsRequired();

    builder.Property(x => x.AssignedAt)
      .HasColumnName("assigned_at")
      .IsRequired();

    builder.Property(x => x.IsActive)
      .HasColumnName("is_active")
      .IsRequired();

    builder
      .HasOne(x => x.PentesterProfile)
      .WithMany()
      .HasForeignKey(x => x.PentesterId)
      .OnDelete(DeleteBehavior.NoAction);
  }
}