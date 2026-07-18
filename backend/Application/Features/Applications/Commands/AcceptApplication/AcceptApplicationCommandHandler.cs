
using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Application.Services;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Applications.Commands;

public sealed class AcceptApplicationCommandHandler : ICommandHandler<AcceptApplicationCommand>
{
  private readonly IApplicationRepository _repo;
  private readonly AssignmentService _assignmentService;
  private readonly IUnitOfWork _unitOfWork;
  public AcceptApplicationCommandHandler(
    IApplicationRepository applicationRepository,
    AssignmentService assignmentService,
    IUnitOfWork unitOfWork
  )
  {
    _repo = applicationRepository;
    _assignmentService = assignmentService;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result> HandleAsync(AcceptApplicationCommand command, CancellationToken ct)
  {
    if (command.ApplicationId <= 0)
      return Result.Failure(new Error(
        DomainErrorCodes.ValidationCodes.ValueOutOfRange,
        "Nie można przekazać takiego ID",
        HttpResponseState.BadRequest
      ));

    var application = await _repo.GetApplicationAsync(command.ApplicationId, ct);

    if (application is null)
      return Result.Failure(new Error(
        DomainErrorCodes.GeneralCodes.NotFound,
        "Nie znaleziono aplikacji",
        HttpResponseState.NotFound
      ));

    if (application.Contract.AuthorId != command.UserId)
      return Result.Failure(new Error(
        DomainErrorCodes.AuthCodes.ForbiddenAccess,
        "Nie jesteś uprawniony do zaakceptowania tej aplikacji",
        HttpResponseState.Forbidden
      ));

    await using var transaction = await _unitOfWork.BeginTransactionAsync();
    await _assignmentService.AssignCandidateToContractAsync(command.UserId, application.ContractId, application.UserId);

    application.Accept();
    application.Contract.StartContract();

    var applicationsToReject = await _repo.GetApplicationsByContractIdAsync(application.ContractId, application.UserId);
    foreach (var app in applicationsToReject)
      app.Reject();

    await _unitOfWork.SaveChangesAsync(ct);
    await transaction.CommitAsync();

    return Result.Success();
  }
}
