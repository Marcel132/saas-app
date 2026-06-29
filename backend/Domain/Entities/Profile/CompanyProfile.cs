using backend.Domain.Entities.Records;

namespace backend.Domain.Entities;

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
  public string? Bio { get; private set; }
  public string? WebsiteUrl { get; private set; }

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

  public void UpdateProfile(
    string? name,
    string? phone,
    string? country,
    string? city,
    string? street,
    string? postalCode,
    string? bio,
    string? websiteUrl)
  {
    if (name is not null)
    {
      if (string.IsNullOrWhiteSpace(name) || name.Length > 256)
        throw new BadRequestAppException();
      Name = name;
    }

    if (phone is not null)
    {
      if (string.IsNullOrWhiteSpace(phone) || phone.Length > 30)
        throw new BadRequestAppException();
      Phone = phone;
    }

    if (country is not null)
    {
      if (string.IsNullOrWhiteSpace(country) || country.Length > 100)
        throw new BadRequestAppException();
      Country = country;
    }

    if (city is not null)
    {
      if (string.IsNullOrWhiteSpace(city) || city.Length > 100)
        throw new BadRequestAppException();
      City = city;
    }

    if (street is not null)
    {
      if (string.IsNullOrWhiteSpace(street) || street.Length > 100)
        throw new BadRequestAppException();
      Street = street;
    }

    if (postalCode is not null)
    {
      if (string.IsNullOrWhiteSpace(postalCode) || postalCode.Length > 10)
        throw new BadRequestAppException();
      PostalCode = postalCode;
    }

    if (bio is not null)
    {
      // Bio jest opcjonalne przy rejestracji, ale jeśli firma aktywnie
      // je ustawia/edytuje w profilu, musi być sensownej długości.
      if (bio.Length > 1000 || bio.Length < 20)
        throw new BadRequestAppException();
      Bio = bio;
    }

    if (websiteUrl is not null)
    {
      if (websiteUrl.Length > 256 || !Uri.IsWellFormedUriString(websiteUrl, UriKind.Absolute))
        throw new BadRequestAppException();
      WebsiteUrl = websiteUrl;
    }
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
    if (data.Bio is not null && data.Bio.Length > 1000)
      throw new BadRequestAppException();
    if (data.WebsiteUrl is not null && (data.WebsiteUrl.Length > 256 || !Uri.IsWellFormedUriString(data.WebsiteUrl, UriKind.Absolute)))
      throw new BadRequestAppException();
  }
}