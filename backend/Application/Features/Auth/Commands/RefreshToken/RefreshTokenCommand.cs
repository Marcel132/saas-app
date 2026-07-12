using backend.Application.Abstractions.CQRS;
using backend.Application.Features.Auth.Dto;

namespace backend.Application.Features.Auth.Commands;

public sealed record RefreshTokenCommand(
  string IpAddress,
  string UserAgent,
  string? RefreshToken
) : ICommand<CredentialsDto>;