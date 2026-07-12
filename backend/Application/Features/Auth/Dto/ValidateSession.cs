using backend.Domain.Entities;

namespace backend.Application.Features.Auth.Dto;

public class ValidateSession
{
  public Guid UserId { get; set; }
  public Session Session { get; set; } = null!;

}