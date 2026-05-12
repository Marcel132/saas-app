public record AuthResult
(
  bool Success,
  Guid userId,
  HashSet<string> Permissions,
  string DomainCode
);


