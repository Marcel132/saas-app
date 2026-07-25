using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Domain.Entities;
using backend.Domain.Entities.Enum;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace backend.Application.Features.Contracts.Commands;

public sealed class ApplyToContractCommandHandler : ICommandHandler<ApplyToContractCommand>
{
  private readonly IContractRepository _repo;
  private readonly IUnitOfWork _unitOfWork;
   private const string PostgresUniqueViolation = "23505";

  public ApplyToContractCommandHandler(
    IContractRepository contractRepository,
    IUnitOfWork unitOfWork
  )
  {
    _repo = contractRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result> HandleAsync(ApplyToContractCommand command, CancellationToken ct)
  {
    if (command.CandidateId == Guid.Empty || command.ContractId <= 0)
      return Result.Failure(new Error(
        DomainCodes.Validation.InvalidValue,
        "Niepoprawne dane wejściowe",
        HttpResponseState.BadRequest
      ));

    var hasApplied = await _repo.HasAlreadyAppliedAsync(command.ContractId, command.CandidateId, ct);
    if (hasApplied)
      return Result.Failure(new Error(
        DomainCodes.General.Conflict,
        "Nie może aplikować dwukrotnie na ten sam kontrakt",
        HttpResponseState.Conflict
      ));

    var contract = await _repo.GetContractByIdAsync(command.ContractId, ct);

    if (contract is null)
      return Result.Failure(new Error(
        DomainCodes.Contract.NotFound,
        "Nie znaleziono kontraktu",
        HttpResponseState.NotFound
      ));

    if (contract.AuthorId == command.CandidateId)
      return Result.Failure(new Error(
        DomainCodes.General.BadRequest,
        "Nie możesz aplikować na własny kontrakt",
        HttpResponseState.BadRequest
      ));

    if(contract.Status != ContractStatus.Open)
      return Result.Failure(new Error(
        DomainCodes.General.BadRequest,
        "Nie możesz aplikować na zamknięty kontrakt",
        HttpResponseState.BadRequest
      ));

    var application = new ContractApplication(command.ContractId, command.CandidateId);
    await _repo.AddApplicationAsync(application);

    try
    {
      await _unitOfWork.SaveChangesAsync(ct);
    }
    catch(DbUpdateException ex) when (ex.InnerException is PostgresException {SqlState: PostgresUniqueViolation})
    {
      return Result.Failure(new Error(
        DomainCodes.General.Conflict,
        "Nie może aplikować dwukrotnie na ten sam kontrakt",
        HttpResponseState.Conflict
      ));
    }

    return Result.Success();
  }
}