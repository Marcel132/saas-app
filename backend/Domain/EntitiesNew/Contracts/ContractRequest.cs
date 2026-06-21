using backend.Domain.EntitiesNew.Enum;

namespace backend.Domain.EntitiesNew;

public class ContractRequest
{
  public long Id { get; private set; }
  public long AssignmentId { get; private set; }
  public string Title { get; private set; } = string.Empty;
  public string Url { get; private set; } = string.Empty;
  public string Scope { get; private set; } = string.Empty;
  public string Credentials { get; private set; } = string.Empty;
  public ContractRequestStatus Status { get; private set; }
  public DateOnly Deadline { get; private set; }
  public bool IsActive { get; private set; }

  private ContractRequest() { } // EF

  public ContractAssignment ContractAssignment { get; private set; } = null!;

  // TODO: Move arguments to dto
  public ContractRequest(long assignmentId, string title, string url, string scope, string credentials, DateOnly deadline)
  {
    if (assignmentId <= 0)
      throw new ValueOutOfRangeAppException("Assignment ID must have value greater than 0");

    if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(scope) || string.IsNullOrWhiteSpace(credentials))
      throw new BadRequestAppException("Błędne dane!");

    if (title.Length > 256)
      throw new BadRequestAppException("Tytuł jest za długi");

    if (string.IsNullOrWhiteSpace(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
      throw new BadRequestAppException("Url ma zły format");

    if (url.Length > 256 || scope.Length > 256)
      throw new BadRequestAppException("Tytuł jest za długi");

    var minimumDeadline = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));

    if (deadline < minimumDeadline)
      throw new BadRequestAppException("Deadline musi być większy niż 7 dni");

    Deadline = deadline;
    AssignmentId = assignmentId;
    Title = title;
    Url = url;
    Scope = scope;
    Credentials = credentials;
    Status = ContractRequestStatus.Created;
    IsActive = true;
  }

  // TODO: Add logic 
  public void StartRequest()
  {
    ChangeStatus(ContractRequestStatus.Testing);
  }

  public void SubmitRequest()
  {
    ChangeStatus(ContractRequestStatus.ReportSubmitted);
  }

  public void CompleteRequest()
  {
    ChangeStatus(ContractRequestStatus.Completed);
  }

  public void Deactivate()
  {
    EnsureIsActive();

    IsActive = false;
  }

  private void EnsureIsActive()
  {
    if (!IsActive)
      throw new BadRequestAppException("Nie można operować na nieaktywnym request");
  }

  private void ChangeStatus(ContractRequestStatus newStatus)
  {
    EnsureIsActive();

    if (!CanModifyStatus(newStatus))
      throw new BadRequestAppException();
    Status = newStatus;
  }
  private bool CanModifyStatus(ContractRequestStatus newStatus)
  {
    return Status switch
    {
      ContractRequestStatus.Created => newStatus == ContractRequestStatus.Testing,
      ContractRequestStatus.Testing => newStatus == ContractRequestStatus.ReportSubmitted,
      ContractRequestStatus.ReportSubmitted => newStatus == ContractRequestStatus.Completed,
      _ => false
    };
  }

}