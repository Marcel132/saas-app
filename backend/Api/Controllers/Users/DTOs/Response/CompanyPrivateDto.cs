using backend.Domain.Entities.Enum;

namespace backend.Api.Controllers.Users.DTOs;

public class CompanyPrivateDto
{
  public Guid Id { get; set; }
  public string Email { get; set; } = string.Empty;
  public RoleType Role { get; set; }
  public bool IsActive { get; set; }
  public DateTime CreatedAt { get; set; }

  public string Nip { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Phone { get; set; } = string.Empty;
  public string Country { get; set; } = string.Empty;
  public string City { get; set; } = string.Empty;
  public string Street { get; set; } = string.Empty;
  public string PostalCode { get; set; } = string.Empty;
  public string? Bio { get; set; } = string.Empty;
  public string? WebsiteUrl { get; set; } = string.Empty;

  public HashSet<string> Roles { get; set; } = [];
  public HashSet<string> Permissions { get; set; } = [];
}