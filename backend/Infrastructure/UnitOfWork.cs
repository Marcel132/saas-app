using backend.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace backend.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
  private readonly AppDbContext _context;
  public UnitOfWork(AppDbContext context)
  {
    _context = context;
  }

  public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

  public async Task<IDbContextTransaction> BeginTransactionAsync() => await _context.Database.BeginTransactionAsync();
}