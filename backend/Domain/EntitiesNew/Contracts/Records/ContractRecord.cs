namespace backend.Domain.Entities.Records;

public record ContractRecord(
  Guid AuthorId,
  string Title,
  string Description,
  decimal PricePerRequest,
  int MaxRequests,
  DateOnly RecruitmentDeadline
);