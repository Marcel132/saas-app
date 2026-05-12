using Microsoft.EntityFrameworkCore;

public class ContractRepository : IContractRepository
{
  private readonly AppDbContext _context;
  public ContractRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task<PagedResponse<ContractResponseDto>> GetContractsAsync(int page, int pageSize, string? search)
  {
    page = Math.Max(page, 1);
    pageSize = Math.Clamp(pageSize, 1, 50);
    var query = _context.Contracts
      .AsNoTracking()
      .Where(c => c.ContractStatus == ContractStatus.Open && c.Deadline > DateTime.UtcNow);
    
    if(!string.IsNullOrEmpty(search))
    {
      query = query
        .Where(c =>
        EF.Functions.ILike(c.Title, $"%{search}%") ||
        EF.Functions.ILike(c.Description, $"%{search}%"));
    }
    var totalItems = await query.CountAsync();

    var contracts = await query
      .OrderByDescending(c => c.CreatedAt)
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
      .Select(c => new ContractResponseDto
      {
        ContractId = c.ContractId,
        AuthorId = c.AuthorId,
        Title = c.Title,
        Price = c.Price,
        Description = c.Description,
        Deadline = c.Deadline,
        CreatedAt = c.CreatedAt,
      })
      .ToListAsync();

    return new PagedResponse<ContractResponseDto>
    {
      Page = page,
      PageSize = pageSize,
      TotalItems = totalItems,
      TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
      Items = contracts
    };
  }
}