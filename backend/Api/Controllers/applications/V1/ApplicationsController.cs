using backend.Api.Auth;
using backend.Api.Http;
using backend.Application.Security;
using backend.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers.Applications.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ApplicationsController : ControllerBase
{
  private readonly IApplicationService _applicationService;
  public ApplicationsController(IApplicationService applicationService)
  {
    _applicationService = applicationService;
  }

  [HttpPatch("{applicationId}/accept")]
  public async Task<IActionResult> AcceptApplication([FromRoute] long applicationId)
  {
    var userId = UserContextExtension.GetUserId(User);
    await _applicationService.AcceptApplicationAsync(userId, applicationId);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "Application accepted successfully.",
      DomainErrorCodes.GeneralCodes.Success
      ));
  }
  
  
  [HttpPatch("{applicationId}/reject")]
  public async Task<IActionResult> RejectApplication([FromRoute] long applicationId)
  {
    var userId = UserContextExtension.GetUserId(User);
    await _applicationService.RejectApplicationAsync(userId, applicationId);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "Application rejected successfully.",
      DomainErrorCodes.GeneralCodes.Success
      ));
  }

  // [HttpGet("me")]
  // public async Task<IActionResult> GetCurrentUserApplications()
  // {
    
  //   var userId = UserContextExtension.GetUserId(User);

  //   var applications = await _applicationService.GetCurrentUserApplications(userId);

  //   return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
  //     HttpContext,
  //     HttpResponseState.Success,
  //     "Applications retrieved successfuly",
  //     DomainErrorCodes.GeneralCodes.Success,
  //     applications
  //   ));
  // }

}