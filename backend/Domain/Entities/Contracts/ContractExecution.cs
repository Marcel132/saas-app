public class ContractExecution
{
  public long ExecutionId { get; private set; }
  public long ContractId { get; private set; }
  public DateTime? StartedAt { get; private set; }
  public DateTime? CompletedAt { get; private set; }
  public ContractExecutionStatus Status { get; private set; } = ContractExecutionStatus.NotStarted;
  public string? ReportUrl { get; private set; }

  private ContractExecution() { } // EF

  public ContractExecution(long contractId)
  {
    if (contractId <= 0)
      throw new ValueOutOfRangeAppException();

    ContractId = contractId;
  }

  public void StartExecution()
  {
    if(Status != ContractExecutionStatus.NotStarted)
      throw new InvalidOperationAppException();
    Status = ContractExecutionStatus.InProgress;
    StartedAt = DateTime.UtcNow;
  }
  public void CompleteExecution(string reportUrl)
  {
    if(Status != ContractExecutionStatus.InProgress)
      throw new InvalidOperationAppException();
    if(string.IsNullOrWhiteSpace(reportUrl) || reportUrl.Length > 1000)
      throw new BadRequestAppException();

    Status = ContractExecutionStatus.Completed;
    CompletedAt = DateTime.UtcNow;
    ReportUrl = reportUrl;
  }
}