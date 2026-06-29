using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Persistence.Config;

public class ContractConfig : IEntityTypeConfiguration<Contract>
{
  public void Configure(EntityTypeBuilder<Contract> builder)
  {
    builder.ToTable("contracts");
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
      .HasColumnName("id")
      .IsRequired();

    builder.Property(x => x.AuthorId)
      .HasColumnName("author_id")
      .IsRequired();

    builder.Property(x => x.Title)
      .HasColumnName("title")
      .HasMaxLength(256)
      .IsRequired();

    builder.Property(x => x.Description)
      .HasColumnName("description")
      .HasMaxLength(1500)
      .IsRequired();

    builder.Property(x => x.PricePerRequest)
      .HasColumnName("price_per_req")
      .IsRequired();

    builder.Property(x => x.MaxRequests)
      .HasColumnName("max_req")
      .IsRequired();

    builder.Ignore(x => x.MaxBudget);

    builder.Property(x => x.Status)
      .HasConversion<string>()
      .HasColumnName("status")
      .IsRequired();

    builder.Property(x => x.IsFunded)
      .HasColumnName("is_funded")
      .IsRequired();

    builder.Property(x => x.CreatedAt)
      .HasColumnName("created_at")
      .IsRequired();

    builder.Property(x => x.UpdatedAt)
      .HasColumnName("updated_at");
      
    builder.Property(x => x.RecruitmentDeadline)
      .HasColumnName("deadline")
      .IsRequired();

    builder.HasIndex(x => x.Status);

    builder
      .HasOne<CompanyProfile>()
      .WithMany()
      .HasForeignKey(x => x.AuthorId)
      .HasPrincipalKey(x => x.UserId)
      .OnDelete(DeleteBehavior.NoAction);

    builder
      .HasMany(x => x.Applications)
      .WithOne(x => x.Contract)
      .HasForeignKey(x => x.ContractId)
      .OnDelete(DeleteBehavior.NoAction);

    builder
      .HasMany(x => x.Assignments)
      .WithOne(x => x.Contract)
      .HasForeignKey(x => x.ContractId)
      .OnDelete(DeleteBehavior.NoAction);
  }
}