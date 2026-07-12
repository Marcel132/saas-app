using backend.Domain.Entities;

namespace backend.Application.Features.Auth.Dto;

public sealed record ValidatedSession(
  Guid UserId,
  Session Session
);