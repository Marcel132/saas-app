namespace backend.Domain.Entities;

public class ContractAssignment
{
  public long Id { get; private set; }
  public long ContractId { get; private set; }
  public Guid DeveloperId { get; private set; }
  public bool IsActive { get; private set; } = true;
  public DateTime AssignedAt { get; private set; } = DateTime.UtcNow;

  private ContractAssignment() { } // EF
  public Contract Contract { get; private set; } = null!;
  public ICollection<ContractRequest> Requests { get; private set; } = [];
  // public ICollection<ContractReport> Reports { get; private set; } = [];

  public ContractAssignment(long contractId, Guid developerId)
  {
    if (contractId <= 0)
      throw new ValueOutOfRangeAppException();
    if (developerId == Guid.Empty)
      throw new BadRequestAppException();

    ContractId = contractId;
    DeveloperId = developerId;
  }

  public void Deactivate()
  {
    IsActive = false;
  }
}