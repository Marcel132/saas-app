using backend.Api.Controllers.Applications.DTOs;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;
using backend.Domain.Interfaces.Features;

namespace backend.Application.Services;

public class ApplicationService : IApplicationService
{
  private readonly IApplicationQueryRepository _applicationQueryRepository;
  private readonly IApplicationRepository _applicationRepository;
  private readonly AssignmentService _assignmentService;
  private readonly IUnitOfWork _unitOfWork;
  public ApplicationService(
    IApplicationQueryRepository applicationQueryRepository,
    IApplicationRepository applicationRepository,
    AssignmentService assignmentService,
    IUnitOfWork unitOfWork
    )
  {
    _applicationQueryRepository = applicationQueryRepository;
    _applicationRepository = applicationRepository;
    _assignmentService = assignmentService;
    _unitOfWork = unitOfWork;
  }

  public async Task AcceptApplicationAsync(Guid userId, long applicationId)
  {

    var application = await _applicationRepository.GetApplicationAsync(applicationId)
      ?? throw new NotFoundAppException("Application not found with provided ID.");

    if (application.Contract.AuthorId != userId)
      throw new UnauthorizedAppException("You are not authorized to accept this application.");

    await using var transaction = await _unitOfWork.BeginTransactionAsync();
    try
    {
      await _assignmentService.AssignCandidateToContractAsync(userId, application.ContractId, application.UserId);
      application.Accept();
      application.Contract.StartContract();

      var applicationsToReject = await _applicationRepository.GetApplicationsByContractIdAsync(application.ContractId, application.UserId);

      foreach (var app in applicationsToReject)
      {
        app.Reject();
      }
      await _unitOfWork.SaveChangesAsync();
      await transaction.CommitAsync();
    }
    catch
    {
      await transaction.RollbackAsync();
      throw;
    }
  }

  public async Task RejectApplicationAsync(Guid userId, long applicationId)
  {
    var application = await _applicationRepository.GetApplicationAsync(applicationId)
      ?? throw new NotFoundAppException("Application not found with provided ID.");

    if (application.Contract.AuthorId != userId)
      throw new UnauthorizedAppException("You are not authorized to reject this application.");

    application.Reject();
    await _unitOfWork.SaveChangesAsync();
  }

  public async Task<List<ApplicationDto>> GetCurrentUserApplications(Guid userId)
  {
    var applications = await _applicationQueryRepository.GetUserApplications(userId);

    return applications;
  }
}