using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Domain.Entities;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Assignments.Commands;

public sealed class AssignCandidateToContractCommandHandler : ICommand<AssignCandidateToContractCommand>
{
  private readonly IAssignmentRepository _assignmentRepository;
  private readonly IReportRepository _reportRepository;
  private readonly IUnitOfWork _unitOfWork;

  public AssignCandidateToContractCommandHandler(
    IUnitOfWork unitOfWork,
    IAssignmentRepository assignmentRepository,
    IReportRepository reportRepository
  )
  {
    _assignmentRepository = assignmentRepository;
    _reportRepository = reportRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result> HandleAsync(AssignCandidateToContractCommand command, CancellationToken ct)
  {
    if (command.ContractId <= 0 || command.DeveloperId == Guid.Empty || command.UserId == Guid.Empty)
      return Result.Failure(new Error(
        DomainErrorCodes.ValidationCodes.MissingRequiredField,
        "Błędne dane",
        HttpResponseState.BadRequest
      ));

    var activeAssignment = await _assignmentRepository.GetActiveAssignmentByContractIdAsync(command.ContractId, ct);

    if(activeAssignment is not null)
      return Result.Failure(new Error(
        DomainErrorCodes.GeneralCodes.Conflict,
        "Wyłączono możliwość przypisania do tego kontraktu",
        HttpResponseState.Conflict
      ));

    
    // TODO: Add navigation between ContractAssignment and ContractReport.
    // TODO: Create both entities before SaveChanges() and remove the extra SaveChanges() call.

    await using var transaction = await _unitOfWork.BeginTransactionAsync();

    var assignment = new ContractAssignment(command.ContractId, command.DeveloperId);
    await _assignmentRepository.AddAssignmentAsync(assignment);

    await _unitOfWork.SaveChangesAsync(ct);

    var report = new ContractReport(assignment.Id);
    await _reportRepository.CreateReport(report);

    await _unitOfWork.SaveChangesAsync(ct);
    await transaction.CommitAsync();

    return Result.Success();
  }
}