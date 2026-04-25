public class Contracts
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

  private Contracts() { } // EF

  public Contracts(Guid authorId, string title, string description, decimal price)
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

  }

  public void UpdateContractDetails(string title, string description)
  {
    if(!CanEditDetails())
      throw new BadRequestAppException();
    if(
      string.IsNullOrWhiteSpace(title) || 
      string.IsNullOrWhiteSpace(description) ||
      title.Length > 255 || description.Length > 1500
      )
      throw new BadRequestAppException();
    
    Title = title;
    Description = description;

    UpdatedAt = DateTime.UtcNow;
  }
  public void UpdatePrice(decimal newPrice)
  {
    if(!CanEditDetails())
      throw new BadRequestAppException();

    if (newPrice <= 0)
      throw new ValueOutOfRangeAppException();

    Price = decimal.Round(newPrice, 2);

    UpdatedAt = DateTime.UtcNow;
  }
  
  // TODO: Divide into multiple methods for different status updates (e.g. StartContract, CompleteContract, CancelContract)
  public void UpdateContractStatus(ContractStatus contractStatus)
  {
    if(ContractStatus == contractStatus)
      return;
    if (!CanContractStatusBeUpdated(contractStatus))
      throw new BadRequestAppException();

    ContractStatus = contractStatus;
    UpdatedAt = DateTime.UtcNow;
  }
  public void ExtendDeadline(DateTime newDeadline)
  {
    if(!CanEditDetails())
      throw new BadRequestAppException();
    if(newDeadline <= DateTime.UtcNow)
      throw new ValueOutOfRangeAppException();
    if (newDeadline <= Deadline)
      throw new ValueOutOfRangeAppException();

    Deadline = newDeadline;
    UpdatedAt = DateTime.UtcNow;
  }
  private bool CanContractStatusBeUpdated(ContractStatus newStatus)
  {
    return ContractStatus switch
    {
      ContractStatus.Open => newStatus == ContractStatus.InProgress || newStatus == ContractStatus.Cancelled,
      ContractStatus.InProgress => newStatus == ContractStatus.Completed || newStatus == ContractStatus.Cancelled,
      ContractStatus.Completed => false,
      ContractStatus.Cancelled => false,
      _ => false
    };
  }
  
  public bool CanEditDetails()
  {
    return ContractStatus == ContractStatus.Open && !IsExpired();
  }
  
  public bool IsExpired()
  {
    return Deadline <= DateTime.UtcNow;
  }

}