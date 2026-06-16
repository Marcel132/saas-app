using backend.Domain.Entities.Enum;

namespace backend.Api.Controllers.Contracts.DTOs;

public class ContractResponseDto
{
  public long ContractId { get; set; }
  public Guid AuthorId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public decimal Price { get; set; }
  public ContractStatus ContractStatus { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  public DateTime Deadline { get; set; }
  public bool HasApplied { get; set; }
}