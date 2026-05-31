public class ContractApplicationsDto
{
  public long ApplicationId { get; set; }
  public long ContractId { get; set; }
  public Guid CandidateId { get; set; }
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string Nickname { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public ContractApplicationStatus Status { get; set; }
  public DateTime AppliedAt { get; set; }
}
