using backend.Domain.Entities.Enum;

namespace backend.Domain.Entities.Records;

public record UserRecord(
  string NormalizedEmail,
  string PasswordHash,
  RoleType Role
);