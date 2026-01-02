public record UserLoginData(
  Guid Id,
  string PasswordHash,
  bool IsActive
);