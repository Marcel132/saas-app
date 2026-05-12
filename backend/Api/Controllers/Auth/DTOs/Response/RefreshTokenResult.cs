public record RefreshTokenResult
(
  bool Success,
  Guid UserId,
  Session? session,
  string DomainCode,
  string? RefreshToken,
  string? AuthToken
);