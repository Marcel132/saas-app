public class ContractApplication
{
  public long ApplicationId { get; private set; }
  public long ContractId { get; private set; }
  public Guid CandidateId { get; private set; }
  public ContractApplicationStatus Status { get; private set; } = ContractApplicationStatus.Pending;
  public DateTime AppliedAt { get; private set; } = DateTime.UtcNow;

  private ContractApplication() { } // EF

  public ContractApplication(long contractId, Guid candidateId)
  {
    if (contractId <= 0)
      throw new ValueOutOfRangeAppException();
    if (candidateId == Guid.Empty)
      throw new BadRequestAppException();

    ContractId = contractId;
    CandidateId = candidateId;
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
    if(!CanModifyStatus(newStatus))
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