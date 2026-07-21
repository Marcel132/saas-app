using backend.Api.Auth;
using backend.Api.Controllers.Users.DTOs;
using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Application.Features.Users.Queries;
using backend.Application.Security;
using backend.Domain.Entities.Enum;
using backend.Domain.Interfaces.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers.Users.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
  private readonly IUserService _userService;
  private readonly IQueryHandler<GetPentesterByIdQuery, UserPublicPentesterDto> _getPentesterById;
  private readonly IQueryHandler<GetCurrentUserQuery, object> _getCurrentUser;
  public UsersController(
    IUserService userService,
    IQueryHandler<GetPentesterByIdQuery, UserPublicPentesterDto> getPentesterById,
    IQueryHandler<GetCurrentUserQuery, object> getCurrentUser
  )
  {
    _userService = userService;
    _getPentesterById = getPentesterById;
    _getCurrentUser = getCurrentUser;
  }

  // GetAllAsync (admin) 

  [HasPermission(Permissions.Users.Read)]
  [HttpGet("{userId}")]
  public async Task<IActionResult> GetPentesterById([FromRoute] Guid userId, CancellationToken ct)
  {
    var currentUserId = UserContextExtension.GetUserId(User);

    var query = new GetPentesterByIdQuery(
      UserId: userId,
      CurrentUserId: currentUserId
    );

    var result = await _getPentesterById.HandleAsync(query, ct);

    if(result.IsFailure)
      return result.ToActionResult(HttpContext);

    return result.ToActionResult(HttpContext);

  }

  [HasPermission(Permissions.Profile.Read)]
  [HttpGet("me")]
  public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
  {
    var userId = UserContextExtension.GetUserId(User);
    var query = new GetCurrentUserQuery(
      UserId: userId
    );

    var result = await _getCurrentUser.HandleAsync(query, ct);

    if(result.IsFailure)
      return result.ToActionResult(HttpContext);

    return result.ToActionResult(
      HttpContext,
      "Pobrano użytkownika",
      DomainErrorCodes.AuthCodes.Success
    );
  }

  [HasPermission(Permissions.Profile.Read)]
  [HttpGet("me/summary")]
  public async Task<IActionResult> GetCurrentUserSummary(CancellationToken ct)
  {
    var userId = UserContextExtension.GetUserId(User);

    var summary = await _userService.GetCurrentUserSummaryAsync(userId, ct);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "Pobrano dane użytkownika",
      DomainErrorCodes.GeneralCodes.Success,
      summary
    ));
  }

  [HasPermission(Permissions.Profile.Update)]
  [HttpPatch("me/pentester")]
  public async Task<IActionResult> UpdateCurrentPentester([FromBody] UpdatePentesterDto request)
  {
    var userId = UserContextExtension.GetUserId(User);
    await _userService.UpdatePentesterAsync(userId, request);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "User updated successfully",
      DomainErrorCodes.AuthCodes.Success
    ));
  }

  [HasPermission(Permissions.Profile.Update)]
  [HttpPatch("me/company")]
  public async Task<IActionResult> UpdateCurrentCompany([FromBody] UpdateCompanyDto request)
  {
    var userId = UserContextExtension.GetUserId(User);
    await _userService.UpdateCompanyAsync(userId, request);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "Company updated successfully",
      DomainErrorCodes.AuthCodes.Success
    ));
  }

  [HasPermission(Permissions.Profile.Delete)]
  [HttpDelete("me")]
  public async Task<IActionResult> DeleteCurrentUser()
  {
    var userId = UserContextExtension.GetUserId(User);
    await _userService.DeleteUserAsync(userId);

    return NoContent();
  }

  [HasPermission(Permissions.Contracts.ReadOwn)]
  [HttpGet("me/contracts")]
  public async Task<IActionResult> GetCurrentUserContracts(CancellationToken ct, [FromQuery] ContractStatus? status = null)
  {
    var userId = UserContextExtension.GetUserId(User);
    var contracts = await _userService.GetCurrentUserContractsAsync(userId, status, ct);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "User contracts retrieved successfully",
      DomainErrorCodes.AuthCodes.Success,
      contracts
    ));
  }

  [HasPermission(Permissions.Applications.ReadOwn)]
  [HttpGet("me/applications")]
  public async Task<IActionResult> GetCurrentUserApplications(CancellationToken ct, [FromQuery] ContractApplicationStatus? status = null)
  {
    var userId = UserContextExtension.GetUserId(User);
    var applications = await _userService.GetCurrentUserApplicationsAsync(userId, status, ct);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "User applications retrieved successfully",
      DomainErrorCodes.AuthCodes.Success,
      applications
    ));
  }
}