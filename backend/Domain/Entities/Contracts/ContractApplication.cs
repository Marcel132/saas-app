using backend.Domain.Entities.Enum;

namespace backend.Domain.Entities;

public class ContractApplication
{
  public long Id { get; private set; }
  public long ContractId { get; private set; }
  public Guid UserId { get; private set; }
  public ContractApplicationStatus Status { get; private set; }
  public DateTime AppliedAt { get; private set; }

  private ContractApplication() { } // EF 

  public Contract Contract { get; private set; } = null!;
  public PentesterProfile PentesterProfile { get; private set; } = null!;

  public ContractApplication(long contractId, Guid userId)
  {
    if(contractId <= 0 || userId == Guid.Empty)
      throw new BadRequestAppException("Nie można utworzyć aplikacji");

    ContractId = contractId;
    UserId = userId;
    Status = ContractApplicationStatus.Pending;
    AppliedAt = DateTime.UtcNow;
  }

  public void Accept()
  {
    ChangeStatus(ContractApplicationStatus.Accepted);
    // Additional logic for accepting application would go here
  }
  public void Reject()
  {
    ChangeStatus(ContractApplicationStatus.Rejected);
    // Additional logic for rejecting application would go here
  }
  private void ChangeStatus(ContractApplicationStatus newStatus)
  {
    if (!CanModifyStatus(newStatus))
      throw new BadRequestAppException();
    Status = newStatus;
  }
  private bool CanModifyStatus(ContractApplicationStatus newStatus)
  {
    return Status switch
    {
      ContractApplicationStatus.Pending => newStatus == ContractApplicationStatus.Accepted || newStatus == ContractApplicationStatus.Rejected,
      ContractApplicationStatus.Accepted => false,
      ContractApplicationStatus.Rejected => false,
      _ => false
    };
  }
}