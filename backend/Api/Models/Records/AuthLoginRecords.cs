public record AuthLoginResult
(
  bool Success,
  Guid userId,
  string HttpCode
);


