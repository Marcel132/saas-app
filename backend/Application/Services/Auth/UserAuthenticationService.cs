using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Application.Features.Auth.Commands;
using backend.Domain.Entities;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Services;

public class UserAuthenticationService
{
  private readonly IUserRepository _users;
  private readonly IPasswordHasher _hasher;
  private readonly ILoginPolicy _policy;
  private readonly IUnitOfWork _unitOfWork;

  private const int MaxAttempts = 5;
  private static readonly TimeSpan BlockDuration = TimeSpan.FromMinutes(10);

  public UserAuthenticationService(
    IUserRepository users,
    IPasswordHasher hasher,
    ILoginPolicy policy,
    IUnitOfWork unitOfWork
  )
  {
    _users = users;
    _hasher = hasher;
    _policy = policy;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<User>> AuthenticateAsync(LoginCommand dto)
  {
    var user = await _users.GetByEmailAsync(dto.Email.Trim().ToLowerInvariant());

    if (user is null)
      return Result<User>.Failure(new Error(
        DomainErrorCodes.AuthCodes.InvalidCredentials,
        "Nieprawidłowe dane",
        HttpResponseState.BadRequest
      ));

    var policyResult = _policy.CanLogin(user);
    if (policyResult.IsFailure)
      return Result<User>.Failure(policyResult.Error);

    if (!_hasher.Verify(dto.Password, user.PasswordHash))
    {
      user.RegisterFailedLoginAttempt(MaxAttempts, BlockDuration);
      await _unitOfWork.SaveChangesAsync();
      return Result<User>.Failure(new Error(
        DomainErrorCodes.AuthCodes.InvalidCredentials,
        "Nieprawidłowe dane",
        HttpResponseState.BadRequest
      ));
    }

    user.ResetFailedLoginAttempts();
    await _unitOfWork.SaveChangesAsync();

    return Result<User>.Success(user);
  }
}