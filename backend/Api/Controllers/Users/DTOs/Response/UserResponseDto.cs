using backend.Domain.Entities.Enum;

namespace backend.Api.Controllers.Users.DTOs;

public class UserResponsePublicDto
{
  public string Nickname { get; set; } = string.Empty;
  public List<Specialization> Specialization { get; set; } = [];
  public string Skills { get; set; } = string.Empty;
  public string? CompanyName { get; set; }
  public DateTime CreatedAt { get; set; }
}

public class UserResponsePrivateDto
{
  public Guid Id { get; set; }
  public string Email { get; set; } = string.Empty;
  public RoleType Role { get; set; }
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string Nickname { get; set; } = string.Empty;
  public List<Specialization> Specialization { get; set; } = [];
  public string Skills { get; set; } = string.Empty;
  public string? CompanyName { get; set; }
  public string? CompanyNip { get; set; }
  public DateTime CreatedAt { get; set; }
  public bool IsActive { get; set; }
  public HashSet<string> Permissions { get; set; } = new();
  public HashSet<string> Roles { get; set; } = new();
}