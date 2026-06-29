using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Persistence.Config;

public class ContractApplicationConfig : IEntityTypeConfiguration<ContractApplication>
{
  public void Configure(EntityTypeBuilder<ContractApplication> builder)
  {
    builder.ToTable("contract_applications");
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
      .HasColumnName("id")
      .IsRequired();

    builder.Property(x => x.ContractId)
      .HasColumnName("contract_id")
      .IsRequired();

    builder.Property(x => x.UserId)
      .HasColumnName("user_id")
      .IsRequired();

    builder.Property(x => x.Status)
      .HasConversion<string>()
      .HasColumnName("status")
      .IsRequired();

    builder.Property(x => x.AppliedAt)
      .HasColumnName("applied_at")
      .IsRequired();

    builder.HasIndex(x => x.Status);

    builder
      .HasOne(x => x.PentesterProfile)
      .WithMany()
      .HasForeignKey(x => x.UserId)
      .OnDelete(DeleteBehavior.NoAction);
  }
}