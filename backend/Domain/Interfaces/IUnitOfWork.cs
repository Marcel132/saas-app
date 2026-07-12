using Microsoft.EntityFrameworkCore.Storage;

namespace backend.Domain.Interfaces;

public interface IUnitOfWork
{
  Task SaveChangesAsync(CancellationToken ct);
  Task<IDbContextTransaction> BeginTransactionAsync();
}