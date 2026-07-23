
using System.Diagnostics.Contracts;
using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Application.Features.Assignments.Commands;
using backend.Application.Services;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Applications.Commands;

public sealed class AcceptApplicationCommandHandler : ICommandHandler<AcceptApplicationCommand>
{
  private readonly ICommandHandler<AssignCandidateToContractCommand> _assignCandidateCommandHandler;
  private readonly IApplicationRepository _repo;
  private readonly IUnitOfWork _unitOfWork;
  public AcceptApplicationCommandHandler(
    ICommandHandler<AssignCandidateToContractCommand> assignCandidateCommandHandler,
    IApplicationRepository applicationRepository,
    IUnitOfWork unitOfWork
  )
  {
    _assignCandidateCommandHandler = assignCandidateCommandHandler;
    _repo = applicationRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result> HandleAsync(AcceptApplicationCommand command, CancellationToken ct)
  {
    if (command.ApplicationId <= 0)
      return Result.Failure(new Error(
        DomainCodes.Validation.ValueOutOfRange,
        "Nie można przekazać takiego ID",
        HttpResponseState.BadRequest
      ));

    var application = await _repo.GetApplicationAsync(command.ApplicationId, ct);

    if (application is null)
      return Result.Failure(new Error(
        DomainCodes.General.NotFound,
        "Nie znaleziono aplikacji",
        HttpResponseState.NotFound
      ));

    if (application.Contract.AuthorId != command.UserId)
      return Result.Failure(new Error(
        DomainCodes.Auth.Forbidden,
        "Nie jesteś uprawniony do zaakceptowania tej aplikacji",
        HttpResponseState.Forbidden
      ));

    await using var transaction = await _unitOfWork.BeginTransactionAsync();

    var assignCommand = new AssignCandidateToContractCommand(
      UserId: command.UserId,
      ContractId: application.ContractId,
      DeveloperId: application.UserId
    );
    var assignment = await _assignCandidateCommandHandler.HandleAsync(assignCommand, ct);

    if(assignment.IsFailure)
      return Result.Failure(assignment.Error);

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
