using backend.Api.Controllers.Users.DTOs;
using backend.Application.Abstractions.CQRS;

namespace backend.Application.Features.Users.Queries;

public sealed record GetUserSummaryQuery(
  Guid UserId
) : IQuery<UserSummaryDto>;