using backend.Domain.Entities;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Services;

public class AssignmentService
{
  private readonly IAssignmentRepository _assignmentRepository;
  public AssignmentService(IAssignmentRepository assignmentRepository)
  {
    _assignmentRepository = assignmentRepository;
  }

  public async Task AssignCandidateToContractAsync(Guid userId, long contractId, Guid developerId)
  {
    if (Guid.Empty == userId || contractId <= 0 || developerId == Guid.Empty)
      throw new ValueOutOfRangeAppException("Invalid input parameters. Please provide valid GUIDs and contract ID."); ;

    var activeAssignment = await _assignmentRepository.GetActiveAssignmentByContractIdAsync(contractId);

    if (activeAssignment != null)
      throw new BadRequestAppException("Contract already has an active assignment.");

    var assignment = new ContractAssignment(contractId, developerId);
    await _assignmentRepository.AddAssignmentAsync(assignment);
  }
}