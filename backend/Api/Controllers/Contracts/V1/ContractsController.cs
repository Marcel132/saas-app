using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ContractsController : ControllerBase
{
  private readonly ContractService _contractService;

  public ContractsController(
    ContractService contractService
  )
  {
    _contractService = contractService;
  }
  
  // -------------------------------
  // path: /contracts         READ
  // -------------------------------

  [HttpGet]
  public async Task<IActionResult> GetContracts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
  {
    var contracts = await _contractService.GetContractsAsync(page, pageSize, search);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      "Contracts retrieved successfully", 
      DomainErrorCodes.AuthCodes.Success,
      contracts
    ));
  }
  
  [HttpGet("{contractId}")]
  public async Task<IActionResult> GetContractById([FromRoute] long contractId)
  {
    var contract = await _contractService.GetContractByIdAsync(contractId);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      "Contract retrieved successfully", 
      DomainErrorCodes.AuthCodes.Success,
      contract
    ));
  }

}