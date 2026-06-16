namespace backend.Application.Services.Auth.DTOs;

public record CredentialsDto(
  string AuthToken,
  string RefreshToken
);