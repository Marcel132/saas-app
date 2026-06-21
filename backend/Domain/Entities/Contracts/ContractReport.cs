using backend.Domain.Entities.Enum;

namespace backend.Domain.Entities;

public class ContractReport
{
  public long Id { get; private set; }
  public long RequestId { get; private set; }
  public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
  public DateTime? ReviewedAt { get; private set; }
  public DateTime? SubmittedAt { get; private set; }
  public DateTime? UpdatedAt { get; private set; }
  public ContractReportStatus Status { get; private set; } = ContractReportStatus.Draft;
  public string? ReportUrl { get; private set; }
  public string? Feedback { get; private set; }


  public ICollection<ContractVulnerability> Vulnerabilities { get; private set; } = [];
  public ContractRequest Request { get; private set; } = null!;
  private ContractReport() { } // EF
  // public ContractAssignment Assignment { get; private set; } = null!;

  public ContractReport(long requestId)
  {
    if (requestId <= 0)
      throw new ValueOutOfRangeAppException();

    RequestId = requestId;
  }

  public void Submit(string reportUrl)
  {
    if (string.IsNullOrWhiteSpace(reportUrl) || reportUrl.Length > 1000)
      throw new ValueOutOfRangeAppException("Report URL is invalid.");

    ChangeStatus(ContractReportStatus.Submitted);

    ReportUrl = reportUrl;
    Feedback = null;
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
    Feedback = null;
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