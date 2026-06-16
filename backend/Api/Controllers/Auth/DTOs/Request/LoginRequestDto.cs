using System.ComponentModel.DataAnnotations;

namespace backend.Api.Controllers.Auth.DTOs;

public class LoginRequestDto
{
  [Required]
  [EmailAddress]
  [StringLength(254)]
  public string Email { get; set; } = string.Empty;

  [Required]
  [StringLength(128, MinimumLength = 8)]
  public string Password { get; set; } = string.Empty;
}