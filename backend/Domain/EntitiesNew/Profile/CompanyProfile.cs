using backend.Domain.Entities.Records;

namespace backend.Domain.EntitiesNew;

public class CompanyProfile
{
  public Guid UserId { get; private set; }
  public string Nip { get; private set; } = string.Empty;
  public string Name { get; private set; } = string.Empty;
  public string Phone { get; private set; } = string.Empty;
  public string Country { get; private set; } = string.Empty;
  public string City { get; private set; } = string.Empty;
  public string Street { get; private set; } = string.Empty;
  public string PostalCode { get; private set; } = string.Empty;
  public string Bio { get; private set; } = string.Empty;
  public string WebsiteUrl { get; private set; } = string.Empty;

  private CompanyProfile() { }

  public User User { get; private set; } = null!;


  public CompanyProfile(Guid userId, CompanyProfileRecord data)
  {
    ValidateData(userId, data);


    UserId = userId;
    Nip = data.Nip;
    Name = data.Name;
    Phone = data.Phone;
    Country = data.Country;
    City = data.City;
    Street = data.Street;
    PostalCode = data.PostalCode;
    Bio = data.Bio;
    WebsiteUrl = data.WebsiteUrl;
  }

  public void Anonymize()
  {
    Nip = string.Empty;
    Name = string.Empty;
    Phone = string.Empty;
    Country = string.Empty;
    City = string.Empty;
    Street = string.Empty;
    PostalCode = string.Empty;
    Bio = string.Empty;
    WebsiteUrl = string.Empty;
  }

  private static void ValidateData(Guid userId, CompanyProfileRecord data)
  {
    if (userId == Guid.Empty)
      throw new BadRequestAppException();
    if (string.IsNullOrWhiteSpace(data.Nip) || data.Nip.Length > 20)
      throw new BadRequestAppException();
    if (string.IsNullOrWhiteSpace(data.Name) || data.Name.Length > 256)
      throw new BadRequestAppException();
    if (string.IsNullOrWhiteSpace(data.Phone) || data.Phone.Length > 30)
      throw new BadRequestAppException();
    if (string.IsNullOrWhiteSpace(data.Country) || data.Country.Length > 100)
      throw new BadRequestAppException();
    if (string.IsNullOrWhiteSpace(data.City) || data.City.Length > 100)
      throw new BadRequestAppException();
    if (string.IsNullOrWhiteSpace(data.Street) || data.Street.Length > 100)
      throw new BadRequestAppException();
    if (string.IsNullOrWhiteSpace(data.PostalCode) || data.PostalCode.Length > 10)
      throw new BadRequestAppException();
    if (string.IsNullOrWhiteSpace(data.Bio) || data.Bio.Length > 1000 || data.Bio.Length < 20)
      throw new BadRequestAppException();
    if (data.WebsiteUrl is not null && data.WebsiteUrl.Length > 256 && !Uri.IsWellFormedUriString(data.WebsiteUrl, UriKind.Absolute))
      throw new BadRequestAppException();
  }
}