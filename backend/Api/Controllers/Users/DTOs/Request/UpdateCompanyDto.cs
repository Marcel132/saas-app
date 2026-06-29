using System.ComponentModel.DataAnnotations;

namespace backend.Api.Controllers.Users.DTOs;

public class UpdateCompanyDto
{
  [StringLength(256)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? Name { get; set; }

  
  [RegularExpression(@"^[^%<>]*$")]
  [StringLength(1000)]
  public string? Bio { get; set; }


  [StringLength(30)]
  [Phone]
  public string? Phone { get; set; }

  [StringLength(100)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? City { get; set; }

  [StringLength(100)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? Country { get; set; }

  [StringLength(10)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? PostalCode { get; set; }

  [StringLength(100)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? Street { get; set; }

  [StringLength(256)]
  [Url]
  public string? WebsiteUrl { get; set; }
}