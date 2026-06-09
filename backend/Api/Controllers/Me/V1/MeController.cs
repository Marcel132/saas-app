using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class MeController : ControllerBase
{
  private readonly MeService _meService;
  
  public MeController(MeService meService)
  {
    _meService = meService;
  }

  [HasPermission(Permissions.ProfileApplications.Read)]
  [HttpGet("applications")]
  public async Task<IActionResult> GetApplications()
  {
    var userId = UserContextExtension.GetUserId(User);

    var applications = await _meService.GetApplications(userId);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "Zwrócono dane aplikacji",
      DomainErrorCodes.GeneralCodes.Success,
      applications
    ));
  }
  
  // [HasPermission(Permissions.ProfileAssignments.Read)]
  // [HttpGet("/me/assignments")]
  // public async Task<IActionResult> GetSelfAssignmentsData()
  // {
  //   return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
  //     HttpContext,
  //     HttpResponseState.Success,
  //     "Zwrócono dane przypisania",
  //     DomainErrorCodes.GeneralCodes.Success
  //   ));
  // }
}