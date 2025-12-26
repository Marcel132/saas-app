using System.ComponentModel.DataAnnotations;
public class RegisterResponseDto
{
  [Required]
  public Guid Id { get; set; }
}