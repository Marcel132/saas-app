using System.ComponentModel.DataAnnotations;

public class UpdateUserDto
{
  [NoHtmlChars]
  public List<Specialization>? SpecializationType { get; set; }
  
  [RegularExpression(@"^[^%<>]*$")]
  [StringLength(1028)]
  public string? Skills { get; set; }
  
  [RegularExpression(@"^[^%<>]*$")]
  [StringLength(64)]
  public string? FirstName { get; set; }
  
  [StringLength(128)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? LastName { get; set; }

  [StringLength(255)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? Nickname { get; set; }
  
  [StringLength(32)]
  [Phone]
  public string? PhoneNumber { get; set; }
  
  [StringLength(128)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? City { get; set; }
  
  [StringLength(64)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? Country { get; set; }
  
  [StringLength(32)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? PostalCode { get; set; }

  [StringLength(128)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? Street { get; set; }
  
  [StringLength(256)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? CompanyName { get; set; }
  
  [StringLength(128)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? CompanyNip { get; set; }
}