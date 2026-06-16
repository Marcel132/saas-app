using backend.Domain.Interfaces.Repositories;

namespace backend.Infrastructure.Persistence.Repositories;

public class MeRepository : IMeRepository
{
  private readonly AppDbContext _context;

  public MeRepository(AppDbContext appDbContext)
  {
    _context = appDbContext;
  }
}