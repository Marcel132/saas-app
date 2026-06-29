using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Persistence.Config;

public class CompanyProfileConfig : IEntityTypeConfiguration<CompanyProfile>
{
  public void Configure(EntityTypeBuilder<CompanyProfile> builder)
  {
    builder.ToTable("profile_companies");
    builder.HasKey(x => x.UserId);

    builder.Property(x => x.UserId)
      .HasColumnName("user_id")
      .IsRequired();
    
    builder.Property(x => x.Nip)
      .HasColumnName("nip")
      .HasMaxLength(20)
      .IsRequired();

    builder.Property(x => x.Name)
      .HasColumnName("name")
      .HasMaxLength(256)
      .IsRequired();
      
     builder.Property(x => x.Phone)
      .HasColumnName("phone")
      .HasMaxLength(30)
      .IsRequired();
    
    builder.Property(x => x.Country)
      .HasColumnName("country")
      .HasMaxLength(100)
      .IsRequired();
    
    builder.Property(x => x.City)
      .HasColumnName("city")
      .HasMaxLength(100)
      .IsRequired();
    
    builder.Property(x => x.Street)
      .HasColumnName("street")
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(x => x.PostalCode)
      .HasColumnName("postal_code")
      .HasMaxLength(10)
      .IsRequired();

    builder.Property(x => x.Bio)
      .HasColumnName("bio")
      .HasMaxLength(1000);

    builder.Property(x => x.WebsiteUrl)
      .HasColumnName("website_url")
      .HasMaxLength(256);
  
    builder.HasIndex(x => x.Nip)
      .IsUnique();

    builder.HasIndex(x => x.Name);
  }
}