public record AuthLoginResult
(
  bool Success,
  Guid userId,
  HashSet<string> Permissions,
  string HttpCode
);


