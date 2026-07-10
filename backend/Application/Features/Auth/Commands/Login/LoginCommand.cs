using backend.Application.Abstractions.CQRS;
using backend.Application.Services.Auth.DTOs;

namespace backend.Application.Features.Auth.Commands;
public sealed record LoginCommand(
  string Email, 
  string Password,
  string IpAddress,
  string UserAgent
) : ICommand<CredentialsDto>;