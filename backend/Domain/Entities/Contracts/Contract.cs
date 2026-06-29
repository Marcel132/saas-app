using backend.Domain.Entities.Records;
using backend.Domain.Entities.Enum;

namespace backend.Domain.Entities;

public class Contract
{
  public long Id { get; private set; }
  public Guid AuthorId { get; private set; }
  public string Title { get; private set; } = string.Empty;
  public string Description { get; private set; } = string.Empty;
  public decimal PricePerRequest { get; private set; }
  public int MaxRequests { get; private set; }
  public decimal MaxBudget => decimal.Round(MaxRequests * PricePerRequest, 2);
  public ContractStatus Status { get; private set; }
  public bool IsFunded { get; private set; }
  public DateTime CreatedAt { get; private set; }
  public DateTime? UpdatedAt { get; private set; }
  public DateOnly RecruitmentDeadline { get; private set; }

  private Contract() { } // EF

  private readonly List<ContractApplication> _applications = [];
  private readonly List<ContractAssignment> _assignments = [];

  public IReadOnlyCollection<ContractApplication> Applications
      => _applications.AsReadOnly();

  public IReadOnlyCollection<ContractAssignment> Assignments
      => _assignments.AsReadOnly();


  public Contract(
    ContractRecord record
  )
  {
    var validated = ValidateData(record);

    AuthorId = validated.AuthorId;
    Title = validated.Title;
    Description = validated.Description;
    PricePerRequest = decimal.Round(validated.PricePerRequest, 2);
    MaxRequests = validated.MaxRequests;
    CreatedAt = DateTime.UtcNow;
    RecruitmentDeadline = validated.RecruitmentDeadline;

    Status = ContractStatus.Open;
  }
  public void StartContract()
  {
    EnsureNotExpired();
    ChangeStatus(ContractStatus.InProgress);
    // TODO: Uncoment this when payments are implemented
    // if(IsFunded == false)
    //   throw new InvalidOperationAppException();
  }
  public void CancelContract()
  {
    ChangeStatus(ContractStatus.Cancelled);
    // Release funds logic would go here
  }
  public void CompleteContract()
  {
    ChangeStatus(ContractStatus.Completed);
    // Release funds logic would go here
  }

  public void UpdateContractDetails(string? title, string? description)
  {
    EnsureCanModifyDetails();


    if (title is not null && title.Length > 256)
      throw new BadRequestAppException("Błędna długość tytułu 0 < tytuł <= 256");
    if (description is not null && description.Length > 1500)
      throw new BadRequestAppException("Błędna długość opisu 0 < tytuł <= 1500");

    if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(description))
      throw new InvalidOperationAppException("Brak danych do aktualizacji");

    Title = string.IsNullOrWhiteSpace(title) ? Title : title;
    Description = string.IsNullOrWhiteSpace(description) ? Description : description;
    UpdatedAt = DateTime.UtcNow;
  }
  public void UpdatePrice(decimal newPrice)
  {
    EnsureCanModifyDetails();

    if (newPrice <= 0)
      throw new ValueOutOfRangeAppException();

    PricePerRequest = decimal.Round(newPrice, 2);
    UpdatedAt = DateTime.UtcNow;
  }
  public void UpdateMaxRequests(int newMaxRequests)
  {
    EnsureCanModifyDetails();

    if (newMaxRequests <= 0)
      throw new ValueOutOfRangeAppException("Maksymalne requesty mają wartość poniżej 0");

    MaxRequests = newMaxRequests;
    UpdatedAt = DateTime.UtcNow;
  }

  public void ChangeDeadline(DateOnly newDeadline)
  {
    EnsureCanModifyDetails();

    var today = DateOnly.FromDateTime(DateTime.UtcNow);
    if (newDeadline <= today)
      throw new BadRequestAppException("Nowy deadline nie może być mniejszy od dzisiejszej daty");

    RecruitmentDeadline = newDeadline;
    UpdatedAt = DateTime.UtcNow;
  }
  public void MarkAsFunded()
  {
    EnsureCanModifyDetails();

    if (IsFunded)
      throw new InvalidOperationAppException();

    IsFunded = true;
    UpdatedAt = DateTime.UtcNow;
  }
  private void EnsureCanModifyDetails()
  {
    EnsureNotExpired();

    if (Status != ContractStatus.Open)
      throw new BadRequestAppException("Nie można edytować kontraktu");
  }
  private void EnsureNotExpired()
  {
    if (RecruitmentDeadline <= DateOnly.FromDateTime(DateTime.UtcNow))
      throw new BadRequestAppException("Nie możesz edytować kontraktu");
  }
  private void ChangeStatus(ContractStatus newStatus)
  {
    if (!CanModifyStatus(newStatus))
      throw new BadRequestAppException();

    Status = newStatus;
    UpdatedAt = DateTime.UtcNow;
  }
  private bool CanModifyStatus(ContractStatus newStatus)
  {
    return Status switch
    {
      ContractStatus.Open => newStatus == ContractStatus.InProgress || newStatus == ContractStatus.Cancelled,
      ContractStatus.InProgress => newStatus == ContractStatus.Completed,
      ContractStatus.Completed => false,
      ContractStatus.Cancelled => false,
      _ => false
    };
  }
  private static ContractRecord ValidateData(ContractRecord r)
  {
    if (r.AuthorId == Guid.Empty)
      throw new BadRequestAppException("Guid jest puste");

    if (string.IsNullOrWhiteSpace(r.Title) || string.IsNullOrWhiteSpace(r.Description))
      throw new BadRequestAppException("Tytuł i opis nie mają wartości");

    if (r.Title.Length > 256 || r.Description.Length > 1500)
      throw new ValueOutOfRangeAppException("Zbyt długi tytuł lub opis");

    if (r.PricePerRequest <= 0 || r.MaxRequests <= 0)
      throw new ValueOutOfRangeAppException("Cena i ilość requestów poniżej 0");

    if (r.RecruitmentDeadline <= DateOnly.FromDateTime(DateTime.UtcNow))
      throw new BadRequestAppException("Niepoprawny deadline ogłoszenia");

    return r with
    {
      AuthorId = r.AuthorId,
      Title = r.Title,
      Description = r.Description,
      PricePerRequest = r.PricePerRequest,
      MaxRequests = r.MaxRequests,
      RecruitmentDeadline = r.RecruitmentDeadline
    };
  }
}