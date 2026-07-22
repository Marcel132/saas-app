using backend.Api.Controllers.Users.DTOs;
using backend.Domain.Entities.Enum;

namespace backend.Domain.Interfaces.Features;

public interface IUserService
{
  // GetAllAsync (admin) 

  public Task UpdatePentesterAsync(Guid userId, UpdatePentesterDto request);
  public Task UpdateCompanyAsync(Guid userId, UpdateCompanyDto request);
  public Task DeleteUserAsync(Guid userId);
}