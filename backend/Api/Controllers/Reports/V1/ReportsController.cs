using backend.Api.Auth;
using backend.Api.Controllers.Reports.DTOs;
using backend.Api.Http;
using backend.Application.Security;
using backend.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers.Reports.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
  private readonly IReportService _reportService;

  public ReportsController(
    IReportService reportService
  )
  {
    _reportService = reportService;
  }

  [HttpGet]
  [HasPermission(Permissions.Reports.Read)]
  public async Task<IActionResult> GetPentesterReports(CancellationToken ct)
  {
    var userId = UserContextExtension.GetUserId(User);

    var reports = await _reportService.GetPentesterReportsAsync(userId, ct);

    return Ok(HttpResponseFactory.CreateSuccessResponse(
      HttpContext,
      HttpResponseState.Success,
      "Reports retrieved successfully",
      DomainErrorCodes.GeneralCodes.Success,
      reports
    ));
  }

  [HttpPost]
  // [HasPermission(Permissions.Reports.Create)]
  public async Task<IActionResult> CreateRequest(CreateRequestDto req)
  {
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Created,
      "Request created",
      DomainErrorCodes.GeneralCodes.Success,
      null
    ));
  }
}