using backend.Api.Auth;
using backend.Api.Controllers.Users.DTOs;
using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Application.Features.Users.Commands;
using backend.Application.Features.Users.Queries;
using backend.Application.Security;
using backend.Domain.Entities.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers.Users.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
  // TODO: Create dispacher for all DI Handlers
  private readonly IQueryHandler<GetPentesterByIdQuery, UserPublicPentesterDto> _getPentesterByIdQueryHandler;
  private readonly IQueryHandler<GetCurrentUserQuery, object> _getCurrentUserQueryHandler;
  private readonly IQueryHandler<GetCurrentUserContractQuery, List<UserContractsDto>> _getCurrentUserContractsQueryHandler;
  private readonly IQueryHandler<GetCurrentUserApplicationsQuery, List<UserApplicationsDto>> _getCurrentUserApplicationsQueryHandler;
  private readonly IQueryHandler<GetUserSummaryQuery, UserSummaryDto> _getSummaryQueryHandler;

  private readonly ICommandHandler<UpdatePentesterCommand> _updatePentesterCommandHandler;
  private readonly ICommandHandler<UpdateCompanyCommand> _updateCompanyCommandHandler;
  private readonly ICommandHandler<DeleteUserCommand> _deleteUserCommandHandler;
  public UsersController(
    IQueryHandler<GetPentesterByIdQuery, UserPublicPentesterDto> getPentesterById,
    IQueryHandler<GetCurrentUserQuery, object> getCurrentUser,
    IQueryHandler<GetCurrentUserContractQuery, List<UserContractsDto>> getCurrentUserContracts,
    IQueryHandler<GetCurrentUserApplicationsQuery, List<UserApplicationsDto>> getCurrentUserApplications,
    IQueryHandler<GetUserSummaryQuery, UserSummaryDto> getSummary,
    ICommandHandler<UpdatePentesterCommand> updatePentester,
    ICommandHandler<UpdateCompanyCommand> updateCompany,
    ICommandHandler<DeleteUserCommand> deleteUser
  )
  {
    _getPentesterByIdQueryHandler = getPentesterById;
    _getCurrentUserQueryHandler = getCurrentUser;
    _getCurrentUserContractsQueryHandler = getCurrentUserContracts;
    _getCurrentUserApplicationsQueryHandler = getCurrentUserApplications;
    _getSummaryQueryHandler = getSummary;
    _updatePentesterCommandHandler = updatePentester;
    _updateCompanyCommandHandler = updateCompany;
    _deleteUserCommandHandler = deleteUser;
  }

  // GetAllAsync (admin) 

  [HasPermission(Permissions.Users.Read)]
  [HttpGet("{userId}")]
  public async Task<IActionResult> GetPentesterById([FromRoute] Guid userId, CancellationToken ct)
  {
    var query = new GetPentesterByIdQuery(
      UserId: userId,
      CurrentUserId: CurrentUserId
    );
    var result = await _getPentesterByIdQueryHandler.HandleAsync(query, ct);

    return result.ToActionResult(
      HttpContext,
      "Pobrano użytkownika",
      DomainCodes.User.Retrieved
      );
  }

  [HasPermission(Permissions.Profile.Read)]
  [HttpGet("me")]
  public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
  {
    var query = new GetCurrentUserQuery(
      UserId: CurrentUserId
    );
    var result = await _getCurrentUserQueryHandler.HandleAsync(query, ct);

    return result.ToActionResult(
      HttpContext,
      "Pobrano użytkownika",
      DomainCodes.User.Retrieved
    );
  }

  [HasPermission(Permissions.Profile.Read)]
  [HttpGet("me/summary")]
  public async Task<IActionResult> GetCurrentUserSummary(CancellationToken ct)
  {
    var query = new GetUserSummaryQuery(
      UserId: CurrentUserId
    );
    var result = await _getSummaryQueryHandler.HandleAsync(query, ct);

    return result.ToActionResult(
      HttpContext,
      "Pobrano wyciąg",
      DomainCodes.User.Retrieved
    );

  }

  [HasPermission(Permissions.Profile.Update)]
  [HttpPatch("me/pentester")]
  public async Task<IActionResult> UpdateCurrentPentester([FromBody] UpdatePentesterDto request, CancellationToken ct)
  {
    var command = new UpdatePentesterCommand(
      UserId: CurrentUserId,
      Dto: request
    );
    var result = await _updatePentesterCommandHandler.HandleAsync(command, ct);

    return result.ToActionResult(
      HttpContext,
      "Zaktualizowano",
      DomainCodes.User.Updated
    );
  }

  [HasPermission(Permissions.Profile.Update)]
  [HttpPatch("me/company")]
  public async Task<IActionResult> UpdateCurrentCompany([FromBody] UpdateCompanyDto request, CancellationToken ct)
  {
    var command = new UpdateCompanyCommand(
      UserId: CurrentUserId,
      Dto: request
    );
    var result = await _updateCompanyCommandHandler.HandleAsync(command, ct);

    return result.ToActionResult(
      HttpContext,
      "Zaktualizowano",
      DomainCodes.User.Updated
    );
  }

  [HasPermission(Permissions.Profile.Delete)]
  [HttpDelete("me")]
  public async Task<IActionResult> DeleteCurrentUser(CancellationToken ct)
  {
    var command = new DeleteUserCommand(
      UserId: CurrentUserId
    );
    var result = await _deleteUserCommandHandler.HandleAsync(command, ct);

    return result.ToActionResult(
      HttpContext,
      "Usunięto użytkownika",
      DomainCodes.User.Deleted
    );
  }

  [HasPermission(Permissions.Contracts.ReadOwn)]
  [HttpGet("me/contracts")]
  public async Task<IActionResult> GetCurrentUserContracts(CancellationToken ct, [FromQuery] ContractStatus? status = null)
  {
    var query = new GetCurrentUserContractQuery(
      UserId: CurrentUserId,
      Status: status
    );
    var result = await _getCurrentUserContractsQueryHandler.HandleAsync(query, ct);

    return result.ToActionResult(
      HttpContext,
      "Kontrakty pobrane",
      DomainCodes.General.Success
    );
  }

  [HasPermission(Permissions.Applications.ReadOwn)]
  [HttpGet("me/applications")]
  public async Task<IActionResult> GetCurrentUserApplications(CancellationToken ct, [FromQuery] ContractApplicationStatus? status = null)
  {
    var query = new GetCurrentUserApplicationsQuery(
      UserId: CurrentUserId,
      Status: status
    );
    var result = await _getCurrentUserApplicationsQueryHandler.HandleAsync(query, ct);

    return result.ToActionResult(
      HttpContext,
      "Pobrano aplikacje",
      DomainCodes.General.Success
    );
  }

  private Guid CurrentUserId => UserContextExtension.GetUserId(User);
}