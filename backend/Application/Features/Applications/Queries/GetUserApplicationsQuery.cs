using backend.Api.Controllers.Applications.DTOs;
using backend.Application.Abstractions.CQRS;

namespace backend.Application.Features.Applications.Queries;

public sealed record GetUserApplicationsQuery(
  Guid UserId
) : IQuery<List<ApplicationDto>>;