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

  // GetAllAsync (admin) 

  [HttpGet("{userId}")]
  public async Task<IActionResult> GetPentesterById([FromRoute] Guid userId)
  {
    var currentUserId = UserContextExtension.GetUserId(User);

    var pentester = await _userService.GetPentesterByIdAsync(userId, currentUserId);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "User retrieved successfully",
      DomainErrorCodes.AuthCodes.Success,
      pentester
    ));
  }

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

  [HttpDelete("me")]
  public async Task<IActionResult> DeleteCurrentUser()
  {
    var userId = UserContextExtension.GetUserId(User);
    await _userService.DeleteUserAsync(userId);

    return NoContent();
  }

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