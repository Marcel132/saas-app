using System.ComponentModel.DataAnnotations;

namespace backend.Api.Controllers.Contracts.DTOs;

public class ContractRequestDto
{
  [Required]
  [StringLength(255)]
  public string Title { get; set; } = string.Empty;

  [Required]
  [StringLength(1500)]
  public string Description { get; set; } = string.Empty;

  [Required]
  public decimal PricePerRequest { get; set; }
  
  [Required]
  public int MaxRequests {get; set; }

  [Required]
  public DateOnly? Deadline { get; set; }
}