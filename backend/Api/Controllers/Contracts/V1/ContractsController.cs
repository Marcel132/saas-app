using backend.Api.Auth;
using backend.Api.Controllers.Contracts.DTOs;
using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Application.Features.Contracts.Queries;
using backend.Application.Security;
using backend.Domain.Interfaces.Features;
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
  private readonly IQueryHandler<GetPublicContractsQuery, PagedResponse<PublicContractDto>> _getPublicContractsQueryHandler;
  private readonly IQueryHandler<GetOpenContractsQuery, PagedResponse<OpenContractDto>> _getOpenContractsQueryHandler;
  private readonly IQueryHandler<GetCompanyContractsQuery, PagedResponse<CompanyContractDto>> _getCompanyContractsQueryHandler;
  private readonly IQueryHandler<GetContractByIdQuery, ContractDetailsDto> _getContractByIdQueryHandler;


  public ContractsController(
    IContractService contractService,
    IQueryHandler<GetPublicContractsQuery, PagedResponse<PublicContractDto>> getPublicContractsQueryHandler,
    IQueryHandler<GetOpenContractsQuery, PagedResponse<OpenContractDto>> getOpenContractsQueryHandler,
    IQueryHandler<GetContractByIdQuery, ContractDetailsDto> getContractByIdQueryHandler,
    IQueryHandler<GetCompanyContractsQuery, PagedResponse<CompanyContractDto>> getCompanyContractsQueryHandler
  )
  {
    _contractService = contractService;
    _getPublicContractsQueryHandler = getPublicContractsQueryHandler;
    _getOpenContractsQueryHandler = getOpenContractsQueryHandler;
    _getContractByIdQueryHandler = getContractByIdQueryHandler;
    _getCompanyContractsQueryHandler = getCompanyContractsQueryHandler;
  }

  [AllowAnonymous]
  [HttpGet("public")]
  public async Task<IActionResult> GetPublicContracts([FromQuery] QueryParams queryParams, CancellationToken ct)
  {
    var query = new GetPublicContractsQuery(
      QueryParams: queryParams
    );

    var result = await _getPublicContractsQueryHandler.HandleAsync(query, ct);

    return result.ToActionResult(
      HttpContext,
      "Pobrano kontrakty {public}",
      DomainCodes.General.Success
    );
  }

  [AllowAnonymous]
  [HttpGet("{contractId:long}")]
  public async Task<IActionResult> GetContractById([FromRoute] long contractId, CancellationToken ct)
  {
    var userId = UserContextExtension.TryGetUserId(User);
    var query = new GetContractByIdQuery(
      ContractId: contractId,
      UserId: userId
    );
    var result = await _getContractByIdQueryHandler.HandleAsync(query, ct);

    return result.ToActionResult(
      HttpContext,
      "Pobrano szczegóły kontraktu",
      DomainCodes.General.Success
    );

  }

  [HasPermission(Permissions.Contracts.Read)]
  [HttpGet]
  public async Task<IActionResult> GetOpenContracts([FromQuery] QueryParams queryParams, CancellationToken ct)
  {
    var userId = UserContextExtension.GetUserId(User);

    var query = new GetOpenContractsQuery(
      UserId: userId,
      QueryParams: queryParams
    );
    var result = await _getOpenContractsQueryHandler.HandleAsync(query, ct);

    return result.ToActionResult(
      HttpContext,
      "Pobrano kontrakty {open}",
      DomainCodes.General.Success
    );
  }

  [HasPermission(Permissions.ContractsSelf.Read)]
  [HttpGet("company")]
  public async Task<IActionResult> GetCompanyContracts([FromQuery] QueryParams queryParams, CancellationToken ct)
  {
    var userId = UserContextExtension.GetUserId(User);
    var query = new GetCompanyContractsQuery(
      UserId: userId, 
      QueryParams: queryParams
    );

    var result = await _getCompanyContractsQueryHandler.HandleAsync(query, ct);

    return result.ToActionResult(
      HttpContext,
      "Pobrano kontrakty {company}",
      DomainCodes.General.Success
    );
  }

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

  [HasPermission(Permissions.Contracts.Update)]
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

  [HasPermission(Permissions.Contracts.Update)]
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

  [HasPermission(Permissions.Contracts.ReadApplications)]
  [HttpGet("{contractId}/applications")]
  public async Task<IActionResult> GetContractApplications([FromRoute] long contractId, CancellationToken ct)
  {
    var userId = UserContextExtension.GetUserId(User);
    var contractApplications = await _contractService.GetContractApplicationsAsync(userId, contractId, ct);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "Contract applications retrieved successfully",
      DomainErrorCodes.AuthCodes.Success,
      contractApplications
    ));
  }

  [HasPermission(Permissions.Contracts.Apply)]
  [HttpPost("{contractId}/apply")]
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

