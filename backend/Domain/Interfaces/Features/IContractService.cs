using backend.Api.Controllers;
using backend.Api.Controllers.Contracts.DTOs;

namespace backend.Domain.Interfaces.Features;

public interface IContractService
{
  public Task<List<ContractApplicationsDto>> GetContractApplicationsAsync(Guid userId, long contractId, CancellationToken ct = default);

  public Task CreateContractAsync(Guid authorId, ContractRequestDto request);
  public Task CloseContractAsync(Guid userId, long contractId);
  public Task UpdateContractAsync(Guid userId, long contractId, UpdateContractDto request);
  public Task ApplyToContractAsync(Guid candidateId, long contractId);
}