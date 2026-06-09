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

  [HttpGet]
  public async Task<IActionResult> GetContracts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
  {
    var userId = UserContextExtension.GetUserId(User);
    var contracts = await _contractService.GetContractsAsync(userId, page, pageSize, search);

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
    var userId = UserContextExtension.GetUserId(User);
    var contract = await _contractService.GetContractByIdAsync(contractId, userId);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      "Contract retrieved successfully", 
      DomainErrorCodes.AuthCodes.Success,
      contract
    ));
  }

  [HasPermission(Permissions.Contracts.Create)]
  [HttpPost]
  public async Task<IActionResult> CreateContract([FromBody] ContractRequestDto contractRequest)
  {
    var userId = UserContextExtension.GetUserId(User);
    
    var contract = await _contractService.CreateContractAsync(userId, contractRequest);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      "Contract created successfully", 
      DomainErrorCodes.AuthCodes.Success,
      contract
    ));
  }

  [HasPermission(Permissions.ContractsSelf.Status)]
  [HttpPatch("{contractId}/close")]
  public async Task<IActionResult> CloseContract([FromRoute] long contractId)
  {
    var userId = UserContextExtension.GetUserId(User);
    await _contractService.CloseContractAsync(userId, contractId);
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      "Contract closed successfully", 
      DomainErrorCodes.AuthCodes.Success
    ));
  }

  [HasPermission(Permissions.ContractsSelf.ModifyDetails)]
  [HttpPatch("{contractId}")]
  public async Task<IActionResult> UpdateContractAsync([FromRoute] long contractId, [FromBody] UpdateContractDto request)
  {
    var userId = UserContextExtension.GetUserId(User);
    await _contractService.UpdateContractAsync(userId, contractId, request);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      "Contract updated successfully", 
      DomainErrorCodes.AuthCodes.Success
    ));
  }

  [HasPermission(Permissions.ContractsSelf.Read)]
  [HttpGet("{contractId}/applications")]
  public async Task<IActionResult> GetContractApplications([FromRoute] long contractId)
  {
    var userId = UserContextExtension.GetUserId(User);
    var contractApplications = await _contractService.GetContractApplicationsAsync(userId, contractId);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      "Contract applications retrieved successfully", 
      DomainErrorCodes.AuthCodes.Success,
      contractApplications
    ));
  }

  [HasPermission(Permissions.ProfileApplications.Create)]
  [HttpPost("{contractId}/applications")]
  public async Task<IActionResult> ApplyToContract([FromRoute] long contractId)
  {
    var userId = UserContextExtension.GetUserId(User);
    await _contractService.ApplyToContractAsync(userId, contractId);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      "Applied to contract successfully", 
      DomainErrorCodes.AuthCodes.Success
    ));
  }
}