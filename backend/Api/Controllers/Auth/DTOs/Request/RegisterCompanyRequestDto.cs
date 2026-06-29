using System.ComponentModel.DataAnnotations;

namespace backend.Api.Controllers.Auth.DTOs;

public class RegisterCompanyRequestDto
{
  [Required]
  [EmailAddress]
  [StringLength(254)]
  public string Email { get; set; } = string.Empty;

  [Required]
  [StringLength(64, MinimumLength = 8)]
  public string Password { get; set; } = string.Empty;

  [Required]
  [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "NIP musi składać się z 10 cyfr")]
  public string Nip { get; set; } = string.Empty;

  [Required]
  [RegularExpression(@"^[^%<>]*$")]
  [StringLength(256)]
  public string Name { get; set; } = string.Empty;

  [Required]
  [RegularExpression(@"^[^%<>]*$")]
  [StringLength(30)]
  [Phone]
  public string Phone { get; set; } = string.Empty;

  [Required]
  [RegularExpression(@"^[^%<>]*$")]
  [StringLength(100)]
  public string City { get; set; } = string.Empty;

  [Required]
  [RegularExpression(@"^[^%<>]*$")]
  [StringLength(100)]
  public string Country { get; set; } = string.Empty;

  [Required]
  [RegularExpression(@"^[^%<>]*$")]
  [StringLength(10)]
  public string PostalCode { get; set; } = string.Empty;

  [Required]
  [RegularExpression(@"^[^%<>]*$")]
  [StringLength(100)]
  public string Street { get; set; } = string.Empty;

  [StringLength(1000)]
  public string? Bio { get; set; }

  [StringLength(256)]
  [Url]
  public string? WebsiteUrl { get; set; }
}