namespace backend.Api.Controllers.Contracts.DTOs;

public class ContractDetailsDto : BaseContractDto
{
  public bool HasApplied { get; set; } = false;
}