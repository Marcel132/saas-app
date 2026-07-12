using backend.Application.Abstractions.CQRS;
using backend.Application.Features.Auth.Dto;

namespace backend.Application.Features.Auth.Commands;

public sealed record RegisterCompanyCommand(
  string Email, 
  string Password,
  string Nip,
  string Name,
  string Phone,
  string City,
  string Country,
  string PostalCode,
  string Street,
  string? Bio,
  string? WebsiteUrl,
  string IpAddress,
  string UserAgent
) : ICommand<CredentialsDto>;