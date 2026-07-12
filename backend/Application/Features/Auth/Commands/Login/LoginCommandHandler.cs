using backend.Application.Abstractions.CQRS;
using backend.Application.Features.Auth.Shared;
using backend.Application.Features.Auth.Dto;

namespace backend.Application.Features.Auth.Commands;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, CredentialsDto>
{
  private readonly UserAuthenticationService _service;
  private readonly AuthCredentialsIssuer _issuer;
  public LoginCommandHandler(
    UserAuthenticationService service,
    AuthCredentialsIssuer issuer
  )
  {
    _service = service;
    _issuer = issuer;
  }

  public async Task<Result<CredentialsDto>> HandleAsync(LoginCommand command, CancellationToken ct)
  {
    var userResult = await _service.AuthenticateAsync(command, ct);

    if(userResult.IsFailure)
      return Result<CredentialsDto>.Failure(userResult.Error);

    var user = userResult.Value;

    var creds =  await _issuer.IssueAsync(user.Id, command.IpAddress, command.UserAgent, ct);
    return creds;
  }
} 