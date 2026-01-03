public class UserData
{
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

  public bool IsEmailVerified { get;  set; }
  public bool IsTwoFactorEnabled { get; set; }
  public bool IsProfileCompleted { get; set;} 

  private UserData() {} // EF

  internal UserData(
    string firstName,
    string lastName,
    string phoneNumber,
    string skills,
    string city,
    string country,
    string postalCode,
    string street,
    string? companyName,
    string? companyNip
    )
  {
    Update(
      firstName, lastName, phoneNumber, skills,
      city, country, postalCode, street,
      companyName, companyNip
    );
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
    if (firstName != null) FirstName = firstName;
    if (lastName != null) LastName = lastName;
    if (phoneNumber != null) PhoneNumber = phoneNumber;
    if (skills != null) Skills = skills;
    if (city != null) City = city;
    if (country != null) Country = country;
    if (postalCode != null) PostalCode = postalCode;
    if (street != null) Street = street;

    CompanyName = companyName;
    CompanyNip  = companyNip;
  }
}