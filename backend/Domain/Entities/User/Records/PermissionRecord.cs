namespace backend.Domain.Entities.Records;

public record PermissionRecord(
  string Action,
  string Resource,
  string Description
);