using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Contracts.Commands;

public sealed class CloseContractCommandHandler : ICommandHandler<CloseContractCommand>
{
  private readonly IContractRepository _repo;
  private readonly IUnitOfWork _unitOfWork;

  public CloseContractCommandHandler(
    IContractRepository contractRepository,
    IUnitOfWork unitOfWork
  )
  {
    _repo = contractRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result> HandleAsync(CloseContractCommand command, CancellationToken ct)
  {
    if (command.ContractId <= 0 || (command.UserId == Guid.Empty))
      return Result.Failure(new Error(
        DomainCodes.Validation.InvalidValue,
        "Niepoprawne dane wejściowe",
        HttpResponseState.BadRequest
      ));

    var contract = await _repo.GetContractByIdAsync(command.ContractId, ct);

    if (contract is null)
      return Result.Failure(new Error(
        DomainCodes.Contract.NotFound,
        "Nie znaleziono",
        HttpResponseState.NotFound
      ));

    if (contract.AuthorId != command.UserId)
      return Result.Failure(new Error(
        DomainCodes.Auth.Forbidden,
        "Nieuprawiony dostęp",
        HttpResponseState.Forbidden
      ));

    contract.CancelContract();
    await _unitOfWork.SaveChangesAsync(ct);
    return Result.Success();
  }
}