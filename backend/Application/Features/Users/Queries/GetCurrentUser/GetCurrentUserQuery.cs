using backend.Application.Abstractions.CQRS;

namespace backend.Application.Features.Users.Queries;

public sealed record GetCurrentUserQuery(
  Guid UserId
) : IQuery<object>;