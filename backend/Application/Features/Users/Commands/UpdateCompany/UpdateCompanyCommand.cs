using backend.Api.Controllers.Users.DTOs;
using backend.Application.Abstractions.CQRS;

namespace backend.Application.Features.Users.Commands;

// TODO: Delete dto from /Application
public sealed record UpdateCompanyCommand(
  Guid UserId,
  UpdateCompanyDto Dto
) : ICommand;