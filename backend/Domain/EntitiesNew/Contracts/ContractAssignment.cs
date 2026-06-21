namespace backend.Domain.EntitiesNew;

public class ContractAssignment
{
  public long Id { get; private set; }
  public long ContractId { get; private set; }
  public Guid PentesterId { get; private set; }
  public DateTime AssignedAt { get; private  set; }
  public bool IsActive { get; private set; } 

  private ContractAssignment() { } // EF

  public Contract Contract { get; private set; } = null!;
  public PentesterProfile PentesterProfile { get; private set; } = null!;

  public ContractAssignment(long contractId, Guid pentesterId)
  {
    if(contractId <= 0 || pentesterId == Guid.Empty)
      throw new BadRequestAppException("Nie można utworzyć assignmentu");

    ContractId = contractId;
    PentesterId = pentesterId;
    AssignedAt = DateTime.UtcNow;
    IsActive = true;
  }

  public void Deactivate()
  {
    if(!IsActive)
      throw new BadRequestAppException("Nie można wyłączyć nieaktywnego przypisania");
    
    IsActive = false;
  }
}