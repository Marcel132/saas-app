public class ApplicationService
{
  private readonly IApplicationRepository _applicationRepository;
  public ApplicationService(IApplicationRepository applicationRepository)
  {
    _applicationRepository = applicationRepository;
  }

  public async Task AcceptApplicationAsync(Guid userId, long applicationId)
  {
    var application =await _applicationRepository.GetApplicationAsync(applicationId)
      ?? throw new NotFoundAppException("Application not found with provided ID.");

    if(application.Contract.AuthorId != userId)
      throw new UnauthorizedAppException();

    application.Accept();
    application.Contract.StartContract();

    // TODO: Add assignment of candidate to contract and other necessary logic for starting the contract

    var applicationsToReject = await _applicationRepository.GetApplicationsByContractIdAsync(application.ContractId, application.CandidateId);

    foreach(var app in applicationsToReject)
    {
      app.Reject();
    }

    await _applicationRepository.SaveChangesAsync();
  }
}