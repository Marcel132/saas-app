using backend.Api.Auth;
using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Application.Features.Applications.Commands;
using backend.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers.Applications.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ApplicationsController : ControllerBase
{
  private readonly ICommandHandler<AcceptApplicationCommand> _acceptCommandHandler;
  private readonly ICommandHandler<RejectApplicationCommand> _rejectCommandHandler;
  public ApplicationsController(
    ICommandHandler<AcceptApplicationCommand> acceptCommandHandler,
    ICommandHandler<RejectApplicationCommand> rejectCommandHandler
  )
  {
    _acceptCommandHandler = acceptCommandHandler;
    _rejectCommandHandler = rejectCommandHandler;
  }

  [HasPermission(Permissions.Applications.Review)]
  [HttpPatch("{applicationId}/accept")]
  public async Task<IActionResult> AcceptApplication([FromRoute] long applicationId, CancellationToken ct)
  {
    var userId = UserContextExtension.GetUserId(User);
    var command = new AcceptApplicationCommand(
      UserId: userId,
      ApplicationId: applicationId
    );
    var result = await _acceptCommandHandler.HandleAsync(command, ct);

    if(result.IsFailure)
      return result.ToActionResult(HttpContext);

    return result.ToActionResult(
      HttpContext,
      "Zaakceptowano aplikację",
      DomainErrorCodes.GeneralCodes.Success
    );
  }
  
  [HasPermission(Permissions.Applications.Review)]
  [HttpPatch("{applicationId}/reject")]
  public async Task<IActionResult> RejectApplication([FromRoute] long applicationId, CancellationToken ct)
  {
    var userId = UserContextExtension.GetUserId(User);
    var command = new RejectApplicationCommand(
      UserId: userId,
      ApplicationId: applicationId
    );

    var result = await _rejectCommandHandler.HandleAsync(command, ct);

    if(result.IsFailure)
      return result.ToActionResult(HttpContext);

    return result.ToActionResult(
      HttpContext,
      "Pomyślnie odrzucono aplikacje",
      DomainErrorCodes.GeneralCodes.Success
    );
  }
}