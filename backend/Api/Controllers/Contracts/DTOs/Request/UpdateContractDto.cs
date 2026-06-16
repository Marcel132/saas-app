namespace backend.Api.Controllers.Contracts.DTOs;

public class UpdateContractDto
{
  public string? Title { get; set; }
  public string? Description { get; set; }
  public decimal? Price { get; set; }
  public DateTime? NewDeadline { get; set; }
}