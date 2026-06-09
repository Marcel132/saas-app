public class MeRepository : IMeRepository
{
  private readonly AppDbContext _appDbContext;

  public MeRepository(AppDbContext appDbContext)
  {
    _appDbContext = appDbContext;
  }
}