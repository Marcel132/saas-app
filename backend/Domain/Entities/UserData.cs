public class UserData
{
  public Guid UserId { get; private set; }

  public string FirstName { get; private set; } = string.Empty;
  public string LastName { get; private set; } = string.Empty;
  public string PhoneNumber { get; private set; } = string.Empty;
  public string Skills { get; private set; } = string.Empty;
  public string City { get; private set; } = string.Empty;
  public string Country { get; private set; } = string.Empty;
  public string PostalCode { get; private set; } = string.Empty;
  public string Street { get; private set; } = string.Empty;

  public string? CompanyName { get; private set; }
  public string? CompanyNip { get; private set; }

  public bool IsEmailVerified { get; private set; }
  public bool IsTwoFactorEnabled { get; private set; }
  public bool IsProfileCompleted { get; private set; }


  private UserData() { } // EF

  public UserData(
    string firstName,
    string lastName,
    string phoneNumber,
    string skills,
    string city,
    string country, 
    string postalCode,
    string street)
  {
    FirstName = firstName;
    LastName = lastName;
    PhoneNumber = phoneNumber;
    Skills = skills;
    City = city;
    Country = country;
    PostalCode = postalCode;
    Street = street;
  }

  public void SetCompanyData(string companyName, string companyNip)
  {
    CompanyName = companyName;
    CompanyNip = companyNip;
  }

  public void Update(
  string? firstName,
  string? lastName,
  string? phoneNumber,
  string? skills,
  string? city,
  string? country,
  string? postalCode,
  string? street,
  string? companyName,
  string? companyNip)
{
  if (!string.IsNullOrWhiteSpace(firstName))
    FirstName = firstName;

  if (!string.IsNullOrWhiteSpace(lastName))
    LastName = lastName;

  if (!string.IsNullOrWhiteSpace(phoneNumber))
    PhoneNumber = phoneNumber;

  if (!string.IsNullOrWhiteSpace(skills))
    Skills = skills;

  if (!string.IsNullOrWhiteSpace(city))
    City = city;

  if (!string.IsNullOrWhiteSpace(country))
    Country = country;

  if (!string.IsNullOrWhiteSpace(postalCode))
    PostalCode = postalCode;

  if (!string.IsNullOrWhiteSpace(street))
    Street = street;

  if (!string.IsNullOrWhiteSpace(companyName))
    CompanyName = companyName;

  if (!string.IsNullOrWhiteSpace(companyNip))
    CompanyNip = companyNip;
}
}