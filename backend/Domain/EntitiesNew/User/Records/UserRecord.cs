using backend.Domain.EntitiesNew.Enum;

namespace backend.Domain.Entities.Records;

public record UserRecord(
  string NormalizedEmail,
  string PasswordHash,
  RoleType Role
);