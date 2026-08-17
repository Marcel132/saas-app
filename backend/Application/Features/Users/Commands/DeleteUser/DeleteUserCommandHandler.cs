using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Features;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Users.Commands;

public sealed class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
  private readonly IUserRepository _repo;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IAuthSessionService _authSessionService;

  public DeleteUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IAuthSessionService authSessionService
  )
  {
    _repo = userRepository;
    _unitOfWork = unitOfWork;
    _authSessionService = authSessionService;
  }

  public async Task<Result> HandleAsync(DeleteUserCommand command, CancellationToken ct)
  {
    var user = await _repo.GetByIdAsync(command.UserId, ct);

    if (user is null)
      return Result.Failure(new Error(
        DomainCodes.User.NotFound,
        "Nie znaleziono użytkownika",
        HttpResponseState.NotFound
      ));

    await using var transaction = await _unitOfWork.BeginTransactionAsync();
      
    user.DeleteAccount();
    await _unitOfWork.SaveChangesAsync(ct);

    await _authSessionService.RevokeAllSessionsAsync(
      userId: user.Id, 
      replaceByTokenId: null, 
      ct: ct
    );

    await transaction.CommitAsync(ct);
    
    return Result.Success();
  }
}