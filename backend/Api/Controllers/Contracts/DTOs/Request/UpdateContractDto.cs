namespace backend.Api.Controllers.Contracts.DTOs;

public class UpdateContractDto
{
  public string? Title { get; set; }
  public string? Description { get; set; }
  public decimal? PricePerRequest { get; set; }
  public int? MaxRequests { get; set; }
  public DateTime? NewDeadline { get; set; }
}