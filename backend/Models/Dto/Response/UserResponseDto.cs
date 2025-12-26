using System.ComponentModel.DataAnnotations;

public class UserResponseDto
{
  [Required]
  public Guid Id { get; set; }
  
  [Required]
  [StringLength(254)]
  public string Email { get; set; } = string.Empty;
  
  [Required]
  [StringLength(64)]
  public string FirstName { get; set; } = string.Empty;

  [Required]
  [StringLength(64)]
  public string LastName { get; set; } = string.Empty;

  [Required]
  [NoHtmlChars]
  public List<string> Specialization { get; set; } = new List<string>();
  
  [Required]
  [StringLength(1028)]
  public string Skills { get; set; } = string.Empty;

  [Required]
  public DateTime CreatedAt { get; set; }
  
  [Required]
  public bool IsActive { get; set; }
}