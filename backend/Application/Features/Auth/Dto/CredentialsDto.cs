namespace backend.Application.Features.Auth.Dto;

public record CredentialsDto(
  string AuthToken,
  string RefreshToken
);