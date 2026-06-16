using backend.Api.Controllers.Auth.DTOs;
using backend.Api.Controllers.Users.DTOs;

namespace backend.Domain.Entities;

public class UserData
{
  public string FirstName { get; private set; } = string.Empty;
  public string LastName { get; private set; } = string.Empty;
  public string Nickname { get; private set; } = string.Empty;
  public string PhoneNumber { get; private set; } = string.Empty;
  public string Description { get; private set; } = string.Empty;
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

  internal UserData(RegisterRequestDto data)
  {
    FirstName = data.FirstName;
    LastName = data.LastName;
    Nickname = data.Nickname;
    PhoneNumber = data.PhoneNumber;
    Description = data.Description;
    City = data.City;
    Country = data.Country;
    PostalCode = data.PostalCode;
    Street = data.Street;

    CompanyName = data.CompanyName;
    CompanyNip = data.CompanyNip;

    IsEmailVerified = false;
    IsTwoFactorEnabled = false;
    IsProfileCompleted = false;
  }

  public void Update(UpdateUserDto data)
  {
    if (data.FirstName != null) FirstName = data.FirstName;
    if (data.LastName != null) LastName = data.LastName;
    if (data.Nickname != null) Nickname = data.Nickname;
    if (data.PhoneNumber != null) PhoneNumber = data.PhoneNumber;
    if (data.Description != null) Description = data.Description;
    if (data.City != null) City = data.City;
    if (data.Country != null) Country = data.Country;
    if (data.PostalCode != null) PostalCode = data.PostalCode;
    if (data.Street != null) Street = data.Street;

    if (data.CompanyName != null && data.CompanyNip != null)
    {
      CompanyName = data.CompanyName;
      CompanyNip = data.CompanyNip;
    }
  }

  public void ClearPersonalData()
  {
    FirstName = string.Empty;
    LastName = string.Empty;
    Nickname = string.Empty;
    PhoneNumber = string.Empty;
    Description = string.Empty;
    City = string.Empty;
    Country = string.Empty;
    PostalCode = string.Empty;
    Street = string.Empty;
    CompanyName = null;
    CompanyNip = null;

    IsEmailVerified = false;
    IsTwoFactorEnabled = false;
    IsProfileCompleted = false;
  }
}