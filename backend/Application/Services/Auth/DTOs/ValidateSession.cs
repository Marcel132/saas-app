using backend.Domain.Entities;

namespace backend.Application.Services.Auth.DTOs;

public class ValidateSession
{
  public Guid UserId { get; set; }
  public Session Session { get; set; } = null!;

}