public class ContractAssignment
{
  public long AssignmentId { get; private set; }
  public long ContractId { get; private set; }
  public Guid DeveloperId { get; private set; }
  public DateTime AssignedAt { get; private set; } = DateTime.UtcNow;

  private ContractAssignment() { } // EF
  public ContractAssignment(long contractId, Guid developerId)
  {
    if (contractId <= 0)
      throw new ValueOutOfRangeAppException();
    if (developerId == Guid.Empty)
      throw new BadRequestAppException();

    ContractId = contractId;
    DeveloperId = developerId;
  }
}