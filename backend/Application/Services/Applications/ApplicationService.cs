public class ApplicationService
{
  private readonly IApplicationRepository _applicationRepository;
  private readonly AssignmentService _assignmentService;
  public ApplicationService(IApplicationRepository applicationRepository, AssignmentService assignmentService)
  {
    _applicationRepository = applicationRepository;
    _assignmentService = assignmentService;
  }

  public async Task AcceptApplicationAsync(Guid userId, long applicationId)
  {
    var application = await _applicationRepository.GetApplicationAsync(applicationId)
      ?? throw new NotFoundAppException("Application not found with provided ID.");

    await _assignmentService.AssignCandidateToContractAsync(userId, application.ContractId, application.CandidateId);
    application.Accept();


    var applicationsToReject = await _applicationRepository.GetApplicationsByContractIdAsync(application.ContractId, application.CandidateId);

    foreach(var app in applicationsToReject)
    {
      app.Reject();
    }

    await _applicationRepository.SaveChangesAsync();
  }

  public async Task RejectApplicationAsync(Guid userId, long applicationId)
  {
    var application = await _applicationRepository.GetApplicationAsync(applicationId)
      ?? throw new NotFoundAppException("Application not found with provided ID.");

    if(application.Contract.AuthorId != userId)
      throw new UnauthorizedAppException();

    application.Reject();
    await _applicationRepository.SaveChangesAsync();
  }
}