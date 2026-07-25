using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Contracts.Commands;

public sealed class UpdateContractCommandHandler : ICommandHandler<UpdateContractCommand>
{
  private readonly IContractRepository _repo;
  private readonly IUnitOfWork _unitOfWork;

  public UpdateContractCommandHandler(
    IContractRepository contractRepository,
    IUnitOfWork unitOfWork
  )
  {
    _repo = contractRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result> HandleAsync(UpdateContractCommand command, CancellationToken ct)
  {
    if (command.UserId == Guid.Empty || command.ContractId <= 0)
      return Result.Failure(new Error(
        DomainCodes.Validation.InvalidValue,
        "Niepoprawne dane wejściowe",
        HttpResponseState.BadRequest
      ));

    var contract = await _repo.GetContractByIdAsync(command.ContractId, ct);

    if (contract is null)
      return Result.Failure(new Error(
        DomainCodes.Contract.NotFound,
        "Nie znaleziono kontraktu",
        HttpResponseState.NotFound
      ));

    if (contract.AuthorId != command.UserId)
      return Result.Failure(new Error(
        DomainCodes.Auth.Forbidden,
        "Nie masz uprawnień do zarządzania kontraktem",
        HttpResponseState.Forbidden
      ));

    await using var transaction = await _unitOfWork.BeginTransactionAsync();
    var request = command.Request;

    if (!string.IsNullOrWhiteSpace(request.Title) || !string.IsNullOrWhiteSpace(request.Description))
      contract.UpdateContractDetails(request.Title, request.Description);

    if (request.PricePerRequest.HasValue)
      contract.UpdatePrice(request.PricePerRequest.Value);

    if (request.MaxRequests.HasValue)
      contract.UpdateMaxRequests(request.MaxRequests.Value);

    if (request.NewDeadline.HasValue)
      contract.ChangeDeadline(DateOnly.FromDateTime(request.NewDeadline.Value));

    await _unitOfWork.SaveChangesAsync(ct);

    await transaction.CommitAsync();

    return Result.Success();
  }
}