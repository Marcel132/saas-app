using Microsoft.EntityFrameworkCore.Storage;

namespace backend.Domain.Interfaces;

public interface IUnitOfWork
{
  Task SaveChangesAsync();
  Task<IDbContextTransaction> BeginTransactionAsync();
}