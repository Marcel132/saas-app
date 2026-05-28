public class AssignmentService
{
  private readonly IAssignmentRepository _assignmentRepository;
  private readonly IContractRepository _contractRepository;
  public AssignmentService(IAssignmentRepository assignmentRepository, IContractRepository contractRepository)
  {
    _assignmentRepository = assignmentRepository;
    _contractRepository = contractRepository;
  }

  public async Task AssignCandidateToContractAsync(Guid userId, long contractId, Guid developerId)
  {
    if(Guid.Empty == userId || contractId <= 0 || developerId == Guid.Empty)
      throw new ValueOutOfRangeAppException("Invalid input parameters. Please provide valid GUIDs and contract ID.");;

    var activeAssignment = await _assignmentRepository.GetActiveAssignmentByContractIdAsync(contractId);

    if (activeAssignment != null)
      throw new BadRequestAppException("Contract already has an active assignment.");

    var assignment = new ContractAssignment(contractId, developerId);
    await _assignmentRepository.AddAssignmentAsync(assignment);

    // ! REMEMBER TO CALL SaveChangesAsync() IN THE SERVICE METHOD THAT CALLS THIS METHOD, OTHERWISE CHANGES WON'T BE PERSISTED TO THE DATABASE
  }
}