using System.ComponentModel.DataAnnotations;

public class ResponseTokenDto
{
  [Required]
  public string AuthToken { get; set; } = string.Empty;
  
  [Required]
  public string RefreshToken { get; set; } = string.Empty;
}