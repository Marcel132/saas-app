using System.Security.Claims;
using backend.Api.Auth;
using backend.Api.Controllers.Contracts.DTOs;
using backend.Api.Http;
using backend.Application.Security;
using backend.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers.Contracts.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ContractsController : ControllerBase
{
  private readonly IContractService _contractService;

  public ContractsController(
    IContractService contractService
  )
  {
    _contractService = contractService;
  }

  [AllowAnonymous]
  [HttpGet("public")]
  public async Task<IActionResult> GetPublicContracts([FromQuery] QueryParams queryParams)
  {
    var contracts = await _contractService.GetPublicContractsAsync(queryParams);

    return Ok(HttpResponseFactory.CreateSuccessResponse(
      HttpContext,
      HttpResponseState.Success,
      "Pobrano publiczne kontrakty",
      DomainErrorCodes.GeneralCodes.Success,
      contracts
    ));
  }

  [HttpGet("company")]
  public async Task<IActionResult> GetCompanyContracts([FromQuery] QueryParams queryParams)
  {
    var userId = UserContextExtension.GetUserId(User);

    var contracts = await _contractService.GetCompanyContractsAsync(userId, queryParams);

    return Ok(HttpResponseFactory.CreateSuccessResponse(
      HttpContext,
      HttpResponseState.Success,
      "Pobrano kontrakty firmy",
      DomainErrorCodes.GeneralCodes.Success,
      contracts
    ));
  }

  [HttpGet]
  public async Task<IActionResult> GetPentesterContracts([FromQuery] QueryParams queryParams)
  {
    var userId = UserContextExtension.GetUserId(User);

    var contracts = await _contractService.GetPentesterContractsAsync(userId, queryParams);

    return Ok(HttpResponseFactory.CreateSuccessResponse(
      HttpContext,
      HttpResponseState.Success,
      "Pobrano kontrakty pentestera",
      DomainErrorCodes.GeneralCodes.Success,
      contracts
    ));
  }
  [AllowAnonymous]
  [HttpGet("{contractId:long}")]
  public async Task<IActionResult> GetContractById([FromRoute] long contractId)
  {
    var userId = UserContextExtension.TryGetUserId(User);

    var contract = await _contractService.GetContractDetailsAsync(contractId, userId);

    return Ok(HttpResponseFactory.CreateSuccessResponse(
      HttpContext,
      HttpResponseState.Success,
      "Pobrano kontrakt",
      DomainErrorCodes.GeneralCodes.Success,
      contract
    ));
  }


  // [HttpGet]
  // public async Task<IActionResult> GetContracts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
  // {
  //   var userId = UserContextExtension.GetUserId(User);
  //   var contracts = await _contractService.GetContractsAsync(userId, page, pageSize, search);

  //   return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
  //     HttpContext,
  //     HttpResponseState.Success,
  //     "Contracts retrieved successfully",
  //     DomainErrorCodes.AuthCodes.Success,
  //     contracts
  //   ));
  // }

  // [HttpGet("{contractId}")]
  // public async Task<IActionResult> GetContractById([FromRoute] long contractId)
  // {
  //   var userId = UserContextExtension.GetUserId(User);
  //   var contract = await _contractService.GetContractByIdAsync(contractId, userId);

  //   return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
  //     HttpContext,
  //     HttpResponseState.Success,
  //     "Contract retrieved successfully",
  //     DomainErrorCodes.AuthCodes.Success,
  //     contract
  //   ));
  // }

  [HasPermission(Permissions.Contracts.Create)]
  [HttpPost]
  public async Task<IActionResult> CreateContract([FromBody] ContractRequestDto contractRequest)
  {
    var userId = UserContextExtension.GetUserId(User);

    await _contractService.CreateContractAsync(userId, contractRequest);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Created,
      "Contract created successfully",
      DomainErrorCodes.AuthCodes.Success
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
      HttpResponseState.Created,
      "Applied to contract successfully",
      DomainErrorCodes.GeneralCodes.Success
    ));
  }
}

