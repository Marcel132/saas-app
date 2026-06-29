namespace backend.Domain.Entities.Records;

public record CompanyProfileRecord(
  string Nip,
  string Name,
  string Phone,
  string Country,
  string City,
  string Street,
  string PostalCode,
  string? Bio,
  string? WebsiteUrl
);