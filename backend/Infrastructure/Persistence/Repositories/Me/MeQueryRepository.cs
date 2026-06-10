using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

  public class MeQueryRepository : IMeQueryRepository
  {
    private readonly AppDbContext _context;

    public MeQueryRepository(AppDbContext appDbContext)
    {
      _context = appDbContext;
    }

  public async Task<List<ApplicationDto>> GetApplicationsAsync(Guid userId)
    {

      return await _context.ContractApplications
        .AsNoTracking()
        .Join(_context.Contracts,
        ca => ca.ContractId,
        c => c.ContractId,
        (ca, c) => new {Application = ca, Contract = c}
        )
        .Where(x => 
          x.Application.CandidateId == userId && 
          (
            x.Application.Status == ContractApplicationStatus.Pending ||
            x.Application.Status == ContractApplicationStatus.Accepted ||
            (
              x.Application.Status == ContractApplicationStatus.Rejected && x.Contract.Deadline > DateTime.UtcNow
            )
          )
        )
        .Select(x => new ApplicationDto
        {
          ApplicationId = x.Application.ApplicationId,
          ContractId = x.Application.ContractId,
          ContractTitle = x.Contract.Title,
          Price = x.Contract.Price,
          AppliedAt = x.Application.AppliedAt,
          ApplicationStatus = x.Application.Status
        })
        .ToListAsync();
    }
  
  public async Task<PagedResponse<ContractResponseDto>> GetContractsAsync(Guid userId, int page, int pageSize, string? search)
  {
    var query = _context.Contracts
      .AsNoTracking()
      .Where(c => c.AuthorId == userId);
    
    if(!string.IsNullOrEmpty(search))
    {
      var escaped = search.Replace(@"\", @"\\").Replace("%", @"\%").Replace("_", @"\_");
      query = query
        .Where(c =>
        EF.Functions.ILike(c.Title, $"%{escaped}%") ||
        EF.Functions.ILike(c.Description, $"%{escaped}%"));
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
        ContractStatus = c.ContractStatus,
        HasApplied = false
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
  public async Task<ContractResponseDto?> GetContractsByIdAsync(long contractId, Guid userId)
  {
    var contract = await _context.Contracts
      .AsNoTracking()
      .Where(c => 
      c.AuthorId == userId &&
      c.ContractId == contractId &&
      c.ContractStatus == ContractStatus.Open &&
      c.Deadline > DateTime.UtcNow
      )
      .Select (c => new ContractResponseDto
      {
        ContractId = c.ContractId,
        AuthorId = c.AuthorId,
        Title = c.Title,
        Description = c.Description,
        Price = c.Price,
        Deadline = c.Deadline,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt,
        ContractStatus = c.ContractStatus,
        HasApplied = false
      })
      .FirstOrDefaultAsync();

    return contract;
  }
  }