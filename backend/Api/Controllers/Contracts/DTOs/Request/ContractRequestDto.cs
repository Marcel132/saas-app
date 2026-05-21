using System.ComponentModel.DataAnnotations;

public class ContractRequestDto
{
  [Required]
  [StringLength(255)]
  public string Title { get; set; } = string.Empty;
  
  [Required]
  [StringLength(1500)]
  public string Description { get; set; } = string.Empty;
  
  [Required]
  public decimal Price { get; set; }
  public DateTime? Deadline { get; set; }
}