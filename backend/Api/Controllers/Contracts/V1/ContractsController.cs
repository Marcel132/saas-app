using backend.Api.Auth;
using backend.Api.Controllers.Contracts.DTOs;
using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Application.Features.Contracts.Commands;
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
  private readonly IQueryHandler<GetPublicContractsQuery, PagedResponse<PublicContractDto>> _getPublicContractsQueryHandler;
  private readonly IQueryHandler<GetOpenContractsQuery, PagedResponse<OpenContractDto>> _getOpenContractsQueryHandler;
  private readonly IQueryHandler<GetCompanyContractsQuery, PagedResponse<CompanyContractDto>> _getCompanyContractsQueryHandler;
  private readonly IQueryHandler<GetContractByIdQuery, ContractDetailsDto> _getContractByIdQueryHandler;
  private readonly IQueryHandler<GetContractApplicationsQuery, List<ContractApplicationsDto>> _getContractApplicationsQueryHandler;

  private readonly ICommandHandler<CreateContractCommand> _createContractCommandHandler;
  private readonly ICommandHandler<CloseContractCommand> _closeContractCommandHandler;
  private readonly ICommandHandler<UpdateContractCommand> _updateContractCommandHandler;
  private readonly ICommandHandler<ApplyToContractCommand> _applyToContractCommand;

  public ContractsController(
    IQueryHandler<GetPublicContractsQuery, PagedResponse<PublicContractDto>> getPublicContractsQueryHandler,
    IQueryHandler<GetOpenContractsQuery, PagedResponse<OpenContractDto>> getOpenContractsQueryHandler,
    IQueryHandler<GetContractByIdQuery, ContractDetailsDto> getContractByIdQueryHandler,
    IQueryHandler<GetCompanyContractsQuery, PagedResponse<CompanyContractDto>> getCompanyContractsQueryHandler,
    IQueryHandler<GetContractApplicationsQuery, List<ContractApplicationsDto>> getContractApplicationsQueryHandler,

    ICommandHandler<CreateContractCommand> createContractCommandHandler,
    ICommandHandler<CloseContractCommand> closeContractCommandHandler,
    ICommandHandler<UpdateContractCommand> updateContractCommandHandler,
    ICommandHandler<ApplyToContractCommand> applyToContractCommand
  )
  {
    _getPublicContractsQueryHandler = getPublicContractsQueryHandler;
    _getOpenContractsQueryHandler = getOpenContractsQueryHandler;
    _getContractByIdQueryHandler = getContractByIdQueryHandler;
    _getCompanyContractsQueryHandler = getCompanyContractsQueryHandler;
    _getContractApplicationsQueryHandler = getContractApplicationsQueryHandler;

    _createContractCommandHandler = createContractCommandHandler;
    _closeContractCommandHandler = closeContractCommandHandler;
    _updateContractCommandHandler = updateContractCommandHandler;
    _applyToContractCommand = applyToContractCommand;
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
      DomainCodes.Contract.Success
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
      DomainCodes.Contract.Success
    );

  }

  [HasPermission(Permissions.Contracts.Read)]
  [HttpGet]
  public async Task<IActionResult> GetOpenContracts([FromQuery] QueryParams queryParams, CancellationToken ct)
  {

    var query = new GetOpenContractsQuery(
      UserId: CurrentUserId,
      QueryParams: queryParams
    );
    var result = await _getOpenContractsQueryHandler.HandleAsync(query, ct);

    return result.ToActionResult(
      HttpContext,
      "Pobrano kontrakty {open}",
      DomainCodes.Contract.Success
    );
  }

  [HasPermission(Permissions.ContractsSelf.Read)]
  [HttpGet("company")]
  public async Task<IActionResult> GetCompanyContracts([FromQuery] QueryParams queryParams, CancellationToken ct)
  {
    var query = new GetCompanyContractsQuery(
      UserId: CurrentUserId, 
      QueryParams: queryParams
    );

    var result = await _getCompanyContractsQueryHandler.HandleAsync(query, ct);

    return result.ToActionResult(
      HttpContext,
      "Pobrano kontrakty {company}",
      DomainCodes.Contract.Success
    );
  }

  [HasPermission(Permissions.Contracts.Create)]
  [HttpPost]
  public async Task<IActionResult> CreateContract([FromBody] ContractRequestDto contractRequest, CancellationToken ct)
  {
    var command = new CreateContractCommand(
      AuthorId: CurrentUserId,
      Request: contractRequest
    );
    var result = await _createContractCommandHandler.HandleAsync(command, ct);

    return result.ToActionResult(
      HttpContext,
      $"Utworzono Kontrakt",
      DomainCodes.Contract.Created
    );
  }

  [HasPermission(Permissions.Contracts.Update)]
  [HttpPatch("{contractId}/close")]
  public async Task<IActionResult> CloseContract([FromRoute] long contractId, CancellationToken ct)
  {
    var command = new CloseContractCommand(
      UserId: CurrentUserId, 
      ContractId: contractId
    );
    var result = await _closeContractCommandHandler.HandleAsync(command, ct);

    return result.ToActionResult(
      HttpContext,
      "Zamknieto kontrakt",
      DomainCodes.Contract.ClosedSuccessfully
    );
  }

  [HasPermission(Permissions.Contracts.Update)]
  [HttpPatch("{contractId}")]
  public async Task<IActionResult> UpdateContractAsync([FromRoute] long contractId, [FromBody] UpdateContractDto request, CancellationToken ct)
  {
    var command = new UpdateContractCommand(
      UserId: CurrentUserId,
      ContractId: contractId,
      Request: request
    );
    var result = await _updateContractCommandHandler.HandleAsync(command, ct);

    return result.ToActionResult(
      HttpContext, 
      "Zaktualizowano Kontrakt",
      DomainCodes.Contract.Updated
    );
  }

  [HasPermission(Permissions.Contracts.ReadApplications)]
  [HttpGet("{contractId}/applications")]
  public async Task<IActionResult> GetContractApplications([FromRoute] long contractId, CancellationToken ct)
  {
    var query = new GetContractApplicationsQuery(
      UserId: CurrentUserId,
      ContractId: contractId
    );

    var result = await _getContractApplicationsQueryHandler.HandleAsync(query, ct);

    return result.ToActionResult(
      HttpContext,
      "Pobrano aplikacje",
      DomainCodes.Contract.Success
    );
  }

  [HasPermission(Permissions.Contracts.Apply)]
  [HttpPost("{contractId}/apply")]
  public async Task<IActionResult> ApplyToContract([FromRoute] long contractId, CancellationToken ct)
  {
    var command = new ApplyToContractCommand(
      CandidateId: CurrentUserId,
      ContractId: contractId
    );

    var result = await _applyToContractCommand.HandleAsync(command, ct);

    return result.ToActionResult(
      HttpContext,
      "Zaaplikowano",
      DomainCodes.Contract.Applied
    );
  }

  private Guid CurrentUserId => UserContextExtension.GetUserId(User);
}

