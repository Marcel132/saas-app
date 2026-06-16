using backend.Api.Auth;
using backend.Api.Controllers.Users.DTOs;
using backend.Api.Http;
using backend.Application.Security;
using backend.Domain.Entities.Enum;
using backend.Domain.Interfaces.Services;
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

  public UsersController(
    IUserService userService
  )
  {
    _userService = userService;
  }

  // * DONE
  [HasPermission(Permissions.Users.ReadAll)]
  [HttpGet]
  public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
  {
    var users = await _userService.GetAllAsync(page, pageSize, search);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "Users retrieved successfully",
      DomainErrorCodes.AuthCodes.Success,
      users
      ));
  }

  // * DONE 
  [HasPermission(Permissions.Users.Read)]
  [HttpGet("{userId}")]
  public async Task<IActionResult> GetUserById([FromRoute] Guid userId)
  {
    var currentUserId = UserContextExtension.GetUserId(User);

    var user = await _userService.GetUserByIdAsync(userId, currentUserId);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "User retrieved successfully",
      DomainErrorCodes.AuthCodes.Success,
      user
    ));
  }

  // * DONE

  [HasPermission(Permissions.Profile.Read)]
  [HttpGet("me")]
  public async Task<IActionResult> GetCurrentUser()
  {
    var userId = UserContextExtension.GetUserId(User);
    var user = await _userService.GetCurrentUserAsync(userId);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "Current user retrieved successfully",
      DomainErrorCodes.AuthCodes.Success,
      user
      ));
  }

  [HasPermission(Permissions.Profile.Read)]
  [HttpGet("me/summary")]
  public async Task<IActionResult> GetCurrentUserSummary()
  {
    var userId = UserContextExtension.GetUserId(User);

    var summary = await _userService.GetCurrentUserSummaryAsync(userId);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "Pobrano dane użytkownika",
      DomainErrorCodes.GeneralCodes.Success,
      summary
    ));
  }

  // * DONE
  [HasPermission(Permissions.Profile.Update)]
  [HttpPatch("me")]
  public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserDto request)
  {
    var userId = UserContextExtension.GetUserId(User);
    await _userService.UpdateUserAsync(userId, request);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "User updated successfully",
      DomainErrorCodes.AuthCodes.Success
    ));
  }

  // * DONE
  [HasPermission(Permissions.Profile.Delete)]
  [HttpDelete("me")]
  public async Task<IActionResult> DeleteCurrentUser()
  {
    var userId = UserContextExtension.GetUserId(User);
    await _userService.DeleteUserAsync(userId);

    return NoContent();
  }

  // * DONE
  [HasPermission(Permissions.ProfileContracts.Read)]
  [HttpGet("me/contracts")]
  public async Task<IActionResult> GetCurrentUserContracts([FromQuery] ContractStatus? status = null)
  {
    var userId = UserContextExtension.GetUserId(User);
    var contracts = await _userService.GetCurrentUserContractsAsync(userId, status);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "User contracts retrieved successfully",
      DomainErrorCodes.AuthCodes.Success,
      contracts
    ));
  }

  // * DONE
  [HasPermission(Permissions.ProfileApplications.Read)]
  [HttpGet("me/applications")]
  public async Task<IActionResult> GetCurrentUserApplications([FromQuery] ContractApplicationStatus? status = null)
  {
    var userId = UserContextExtension.GetUserId(User);
    var applications = await _userService.GetCurrentUserApplicationsAsync(userId, status);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "User applications retrieved successfully",
      DomainErrorCodes.AuthCodes.Success,
      applications
    ));
  }
}
