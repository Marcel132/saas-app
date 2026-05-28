using Microsoft.EntityFrameworkCore.Storage;
public interface IUnitOfWork
{
  Task SaveChangesAsync();
  Task<IDbContextTransaction> BeginTransactionAsync();
}