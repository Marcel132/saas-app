public class Contract
{
  public long ContractId { get; private set; }
  public Guid AuthorId { get; private set; }
  public string Title { get; private set; } = string.Empty;
  public string Description { get; private set; } = string.Empty;
  public decimal Price { get; private set; }
  public ContractStatus ContractStatus { get; private set; } // * For status use only ContractEnum
  public bool IsFunded { get; private set; }
  public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; private set; }
  public DateTime Deadline { get; private set; } = DateTime.UtcNow.AddDays(30);

  private Contract() { } // EF
  public ICollection<ContractAssignment> Assignments { get; private set; } = [];

  public Contract(Guid authorId, string title, string description, decimal price, DateTime? deadline = null)
  {
    if(authorId == Guid.Empty)
      throw new BadRequestAppException();
    if(
      string.IsNullOrWhiteSpace(title) || 
      string.IsNullOrWhiteSpace(description) ||
      title.Length > 255 || description.Length > 1500
      )
      throw new BadRequestAppException();
    if (price <= 0)
      throw new ValueOutOfRangeAppException();

  
    AuthorId = authorId;
    Title = title;
    Description = description;
    Price = decimal.Round(price, 2);
    ContractStatus = ContractStatus.Open;

    if(deadline != null)
    {
      if(deadline.Value <= DateTime.UtcNow)
        throw new ValueOutOfRangeAppException();
      Deadline = deadline.Value;
    }

  }

  public void StartContract()
  {
    if(IsExpired())
      throw new InvalidOperationAppException();
    // TODO: Uncoment this when payments are implemented
    // if(IsFunded == false)
    //   throw new InvalidOperationAppException();
    ChangeStatus(ContractStatus.InProgress);
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
    if(!CanModifyDetails())
      throw new BadRequestAppException();
    if(string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(description))
      throw new InvalidOperationAppException();
    if(title?.Length > 255 || description?.Length > 1500)
      throw new BadRequestAppException();

    Title = string.IsNullOrWhiteSpace(title) ? Title : title;
    Description = string.IsNullOrWhiteSpace(description) ? Description : description;
    UpdatedAt = DateTime.UtcNow;
  }
  public void UpdatePrice(decimal newPrice)
  {
    if(!CanModifyDetails())
      throw new BadRequestAppException();
    if (newPrice <= 0)
      throw new ValueOutOfRangeAppException();

    Price = decimal.Round(newPrice, 2);
    UpdatedAt = DateTime.UtcNow;
  }
  public bool IsExpired()
  {
    return Deadline <= DateTime.UtcNow;
  }
  public bool CanModifyDetails()
  {
    return ContractStatus == ContractStatus.Open && !IsExpired();
  }
  public void ExtendDeadline(DateTime newDeadline)
  {
    if(!CanModifyDetails())
      throw new BadRequestAppException();
    if(newDeadline <= DateTime.UtcNow)
      throw new ValueOutOfRangeAppException();
    if (newDeadline <= Deadline)
      throw new ValueOutOfRangeAppException();

    Deadline = newDeadline;
    UpdatedAt = DateTime.UtcNow;
  }
  public void MarkAsFunded()
  {
    if(IsFunded)
      throw new InvalidOperationAppException();

    if(ContractStatus != ContractStatus.Open)
      throw new InvalidOperationAppException();

    if(IsExpired())
      throw new InvalidOperationAppException();

    IsFunded = true;
    UpdatedAt = DateTime.UtcNow;
  }
 
  private void ChangeStatus(ContractStatus newStatus)
  {
    if(!CanModifyStatus(newStatus))
      throw new BadRequestAppException();

    ContractStatus = newStatus;
    UpdatedAt = DateTime.UtcNow;
  }
  private bool CanModifyStatus(ContractStatus newStatus)
  {
    return ContractStatus switch
    {
      ContractStatus.Open => newStatus == ContractStatus.InProgress || newStatus == ContractStatus.Cancelled,
      ContractStatus.InProgress => newStatus == ContractStatus.Completed,
      ContractStatus.Completed => false,
      ContractStatus.Cancelled => false,
      _ => false
    };
  }



}