public class UserApplicationsDto
{
  public long ApplicationId { get; set; }
  public long ContractId { get; set; }
  public Guid CompanyId { get; set; }
  public ContractApplicationStatus Status { get; set; }
  public DateTime AppliedAt { get; set; }
}