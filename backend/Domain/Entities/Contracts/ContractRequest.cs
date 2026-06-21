using backend.Domain.Entities.Enum;

namespace backend.Domain.Entities;

public class ContractRequest
{
  public long Id { get; private set; }
  public long AssignmentId { get; private set; }
  public string Title { get; private set; } = string.Empty;
  public string Url { get; private set; } = string.Empty;
  public string Scope { get; private set; } = string.Empty;
  public string Credentials { get; private set; } = string.Empty;
  public DateTime? Deadline { get; private set; } = DateTime.UtcNow.AddDays(7);

  public ContractRequestStatus Status { get; private set; } = ContractRequestStatus.Created;

  public ContractAssignment ContractAssignment { get; private set; } = null!;

  private ContractRequest() { } // EF

  // TODO: Move arguments to dto
  public ContractRequest(long assignmentId, string title, string url, string scope, string credentials, DateTime? deadline)
  {
    if (assignmentId <= 0)
      throw new ValueOutOfRangeAppException("Assignment ID must have value greater than 0");

    if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(scope) || string.IsNullOrWhiteSpace(credentials))
      throw new BadRequestAppException("Błędne dane!");

    if (deadline.HasValue && deadline <= DateTime.UtcNow.AddDays(7))
      throw new BadRequestAppException("Deadline nie może być mniejszy niż 7 dni");

    Deadline = deadline;
    AssignmentId = assignmentId;
    Title = title;
    Url = url;
    Scope = scope;
    Credentials = credentials;
  }

  // TODO: Add logic 
  public void StartRequest()
  {
    Status = ContractRequestStatus.Testing;
  }

  public void SubmitRequest()
  {
    Status = ContractRequestStatus.ReportSubmitted;
  }

  public void CompleteRequest()
  {
    Status = ContractRequestStatus.Completed;
  }

}