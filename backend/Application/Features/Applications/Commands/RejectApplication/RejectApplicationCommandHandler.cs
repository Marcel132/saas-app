using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Applications.Commands;

public sealed class RejectApplicationCommandHandler : ICommandHandler<RejectApplicationCommand>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IApplicationRepository _repo;
  public RejectApplicationCommandHandler(
    IUnitOfWork unitOfWork,
    IApplicationRepository applicationRepository
  )
  {
    _unitOfWork = unitOfWork;
    _repo = applicationRepository;
  }

  public async Task<Result> HandleAsync(RejectApplicationCommand command, CancellationToken ct)
  {
    if (command.ApplicationId <= 0)
      return Result.Failure(new Error(
        DomainCodes.Validation.ValueOutOfRange,
        "Nie można przekazać takiego ID",
        HttpResponseState.BadRequest
      ));
    
    var application = await _repo.GetApplicationAsync(command.ApplicationId, ct);

    if(application is null)
      return Result.Failure(new Error(
        DomainCodes.General.NotFound,
        $"Nie znaleziono aplikacji o podanym ID ({command.ApplicationId})",
        HttpResponseState.BadRequest
      ));

    if(application.Contract.AuthorId != command.UserId)
      return Result.Failure(new Error(
        DomainCodes.Auth.Forbidden,
        "Nie jesteś uprawniony do zaakceptowania tej aplikacji",
        HttpResponseState.Forbidden
      ));
    
    application.Reject();
    await _unitOfWork.SaveChangesAsync(ct);

    return Result.Success();
  }
}