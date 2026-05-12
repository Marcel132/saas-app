public record ContractResult
(
  bool Success,
  string Message,
  string Code,
  IReadOnlyList<PagedResponse<ContractResponseDto>> Contracts
);