using backend.Domain.Entities.Enum;

namespace backend.Domain.Entities;

public class ContractReport
{
  public long Id { get; private set; }
  public long AssignmentId { get; private set; }
  public ContractReportStatus Status { get; private set; }
  public string? Feedback { get; private set; }
  public DateTime CreatedAt { get; private set; } 
  public DateTime? ReviewedAt { get; private set; } 
  public DateTime? SubmittedAt { get; private set; } 
  public DateTime? UpdatedAt { get; private set; } 

  private ContractReport() { } // EF
  public ContractAssignment ContractAssignment { get; private set; } = null!;

  public ContractReport(long assignmentId)
  {
    if(assignmentId <= 0)
      throw new BadRequestAppException();

    AssignmentId = assignmentId;
    Status = ContractReportStatus.Draft;
    CreatedAt = DateTime.UtcNow;
  }

public void Submit()
  {
    ChangeStatus(ContractReportStatus.Submitted);

    SubmittedAt = DateTime.UtcNow;
  }

  public void RequestChanges(string feedback)
  {
    if (string.IsNullOrWhiteSpace(feedback) || feedback.Trim().Length <= 20 || feedback.Length > 2000)
      throw new ValueOutOfRangeAppException("Feedback is invalid.");

    ChangeStatus(ContractReportStatus.ChangesRequested);

    Feedback = feedback;
    ReviewedAt = DateTime.UtcNow;
  }

  public void Approve()
  {
    ChangeStatus(ContractReportStatus.Approved);
    ReviewedAt = DateTime.UtcNow;
  }

  public void Reject(string feedback)
  {
    if (string.IsNullOrWhiteSpace(feedback) || feedback.Trim().Length <= 20 || feedback.Length > 2000)
      throw new ValueOutOfRangeAppException("Feedback is invalid.");

    ChangeStatus(ContractReportStatus.Rejected);

    Feedback = feedback;
    ReviewedAt = DateTime.UtcNow;
  }

  private void ChangeStatus(ContractReportStatus newStatus)
  {
    if (!CanModifyStatus(newStatus))
      throw new InvalidOperationAppException($"Cannot change report status from {Status} to {newStatus}.");

    Status = newStatus;
    UpdatedAt = DateTime.UtcNow;
  }
  private bool CanModifyStatus(ContractReportStatus newStatus)
  {
    return Status switch
    {
      ContractReportStatus.Draft => newStatus == ContractReportStatus.Submitted,
      ContractReportStatus.Submitted => newStatus == ContractReportStatus.Approved || newStatus == ContractReportStatus.Rejected || newStatus == ContractReportStatus.ChangesRequested,
      ContractReportStatus.ChangesRequested => newStatus == ContractReportStatus.Submitted,
      ContractReportStatus.Rejected => newStatus == ContractReportStatus.Submitted,
      ContractReportStatus.Approved => false,
      _ => false
    };

  }
}