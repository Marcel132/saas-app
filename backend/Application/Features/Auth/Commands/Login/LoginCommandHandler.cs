using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Application.Services;
using backend.Application.Services.Auth;
using backend.Application.Services.Auth.DTOs;

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

  public async Task<Result<CredentialsDto>> HandleAsync(LoginCommand command)
  {
    var userResult = await _service.AuthenticateAsync(command);

    if(userResult.IsFailure)
      return Result<CredentialsDto>.Failure(userResult.Error);

    var user = userResult.Value;

    var creds =  await _issuer.IssueAsync(user.Id, command.IpAddress, command.UserAgent);
    return creds;
  }
} 