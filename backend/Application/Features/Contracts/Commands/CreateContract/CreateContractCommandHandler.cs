using backend.Application.Abstractions.CQRS;
using backend.Domain.Entities;
using backend.Domain.Entities.Records;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Contracts.Commands;

public sealed class CreateContractCommandHandler : ICommandHandler<CreateContractCommand>
{
  private readonly IContractRepository _repo;
  private readonly IUnitOfWork _unitOfWork;
  public CreateContractCommandHandler(
    IContractRepository contractRepository,
    IUnitOfWork unitOfWork
  )
  {
    _repo = contractRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result> HandleAsync(CreateContractCommand command, CancellationToken ct)
  {
    var request = command.Request;

    var data = new ContractRecord(
      AuthorId: command.AuthorId,
      Title: request.Title,
      Description: request.Description,
      PricePerRequest: request.PricePerRequest,
      MaxRequests: request.MaxRequests,
      RecruitmentDeadline: request.Deadline ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30))
    );

    var contract = new Contract(data);
    await _repo.AddContractAsync(contract);
    await _unitOfWork.SaveChangesAsync(ct);

    return Result.Success();
  }
}