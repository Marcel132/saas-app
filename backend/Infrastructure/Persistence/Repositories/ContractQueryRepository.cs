using backend.Api.Controllers;
using backend.Api.Controllers.Contracts.DTOs;
using backend.Domain.Entities;
using backend.Domain.Entities.Enum;
using backend.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Infrastructure.Persistence.Repositories;

public class ContractQueryRepository : IContractQueryRepository
{
  private readonly AppDbContext _context;
  public ContractQueryRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task<PagedResponse<PublicContractDto>> GetPublicContractsAsync(QueryParams queryParams, CancellationToken ct)
  {
    var query = _context.Contracts
      .AsNoTracking()
      .Where(c =>
        c.Status == ContractStatus.Open &&
        c.RecruitmentDeadline > DateOnly.FromDateTime(DateTime.UtcNow)
      );

    query = ApplySearch(query, queryParams.Search);

    var totalItems = await query.CountAsync(ct);

    var contracts = await query
      .OrderByDescending(c => c.CreatedAt)
      .Skip((queryParams.Page - 1) * queryParams.PageSize)
      .Take(queryParams.PageSize)
      .Select(c => new PublicContractDto
      {
        ContractId = c.Id,
        Title = c.Title,
        Description = c.Description,
        ContractStatus = c.Status,
        CreatedAt = c.CreatedAt,
        Deadline = c.RecruitmentDeadline,
        PricePerRequest = c.PricePerRequest,
        MaxBudget = c.MaxBudget,
        MaxRequests = c.MaxRequests,
        UpdatedAt = c.UpdatedAt
      })
      .ToListAsync(ct);

    return new PagedResponse<PublicContractDto>
    {
      Page = queryParams.Page,
      PageSize = queryParams.PageSize,
      TotalItems = totalItems,
      TotalPages = (int)Math.Ceiling(totalItems / (double)queryParams.PageSize),
      Items = contracts
    };
  }

  public async Task<PagedResponse<OpenContractDto>> GetOpenContractsAsync(Guid userId, QueryParams queryParams, CancellationToken ct)
  {
    var query = _context.Contracts
      .AsNoTracking()
      .Where(c =>
        c.Status == ContractStatus.Open &&
        c.RecruitmentDeadline > DateOnly.FromDateTime(DateTime.UtcNow)
      );

    query = ApplySearch(query, queryParams.Search);

    var totalItems = await query.CountAsync(ct);

    var contracts = await query
     .OrderByDescending(c => c.CreatedAt)
     .Skip((queryParams.Page - 1) * queryParams.PageSize)
     .Take(queryParams.PageSize)
     .Select(c => new OpenContractDto
     {
       ContractId = c.Id,
       Title = c.Title,
       Description = c.Description,
       ContractStatus = c.Status,
       CreatedAt = c.CreatedAt,
       Deadline = c.RecruitmentDeadline,
       PricePerRequest = c.PricePerRequest,
       MaxBudget = c.MaxBudget,
       MaxRequests = c.MaxRequests,
       UpdatedAt = c.UpdatedAt,
       HasApplied = _context.ContractApplications
         .Any(ca =>
           ca.ContractId == c.Id &&
           ca.UserId == userId &&
           ca.Status == ContractApplicationStatus.Pending
         )
     })
     .ToListAsync(ct);

    return new PagedResponse<OpenContractDto>
    {
      Page = queryParams.Page,
      PageSize = queryParams.PageSize,
      TotalItems = totalItems,
      TotalPages = (int)Math.Ceiling(totalItems / (double)queryParams.PageSize),
      Items = contracts
    };
  }

  public async Task<PagedResponse<CompanyContractDto>> GetCompanyContractsAsync(Guid userId, QueryParams queryParams, CancellationToken ct)
  {
    var query = _context.Contracts
      .AsNoTracking()
      .Where(c => c.AuthorId == userId);

    query = ApplySearch(query, queryParams.Search);

    var totalItems = await query.CountAsync(ct);

    var contracts = await query
      .OrderBy(c =>
        c.Status == ContractStatus.InProgress ? 0 :
        c.Status == ContractStatus.Open ? 1 :
        c.Status == ContractStatus.Completed ? 2 :
        3
      )
      .ThenByDescending(c => c.CreatedAt)
      .Skip((queryParams.Page - 1) * queryParams.PageSize)
      .Take(queryParams.PageSize)
      .Select(c => new CompanyContractDto
      {
        ContractId = c.Id,
        ContractStatus = c.Status,
        CreatedAt = c.CreatedAt,
        Deadline = c.RecruitmentDeadline,
        Description = c.Description,
        PricePerRequest = c.PricePerRequest,
        MaxBudget = c.MaxBudget,
        MaxRequests = c.MaxRequests,
        Title = c.Title,
        UpdatedAt = c.UpdatedAt,
        NumberOfApplications = _context.ContractApplications
          .Count(ca => ca.ContractId == c.Id)
      })
      .ToListAsync(ct);

    return new PagedResponse<CompanyContractDto>
    {
      Page = queryParams.Page,
      PageSize = queryParams.PageSize,
      TotalItems = totalItems,
      TotalPages = (int)Math.Ceiling(totalItems / (double)queryParams.PageSize),
      Items = contracts
    };
  }

  public async Task<ContractDetailsDto?> GetContractDetailsAsync(long contractId, Guid? userId, CancellationToken ct)
  {
    var query = _context.Contracts
      .AsNoTracking()
      .Where(c => c.Id == contractId);

    if (userId.HasValue)
    {
      query = query
        .Where(c =>
          c.AuthorId == userId ||
          (
            c.Status == ContractStatus.Open &&
            c.RecruitmentDeadline > DateOnly.FromDateTime(DateTime.UtcNow)
          )
        );
    }
    else
    {
      query = query.Where(c => c.Status == ContractStatus.Open);
    }

    return await query
      .Select(c => new ContractDetailsDto
      {
        ContractId = c.Id,
        ContractStatus = c.Status,
        CreatedAt = c.CreatedAt,
        Deadline = c.RecruitmentDeadline,
        Description = c.Description,
        PricePerRequest = c.PricePerRequest,
        MaxBudget = c.MaxBudget,
        MaxRequests = c.MaxRequests,
        Title = c.Title,
        UpdatedAt = c.UpdatedAt,
        HasApplied = userId.HasValue && _context.ContractApplications
          .Any(ca =>
            ca.ContractId == c.Id &&
            ca.UserId == userId.Value &&
            ca.Status == ContractApplicationStatus.Pending
          )
      })
      .FirstOrDefaultAsync(ct);
  }

  public async Task<List<ContractApplicationsDto>> GetContractApplicationsAsync(long contractId, CancellationToken ct)
  {
    return await _context.ContractApplications
      .AsNoTracking()
      .Where(ca => ca.ContractId == contractId)
      .Join(
        _context.Users,
        ca => ca.UserId,
        u => u.Id,
        (ca, u) => new { ca, u }
      )
      .Where(x => x.u.PentesterProfile != null)
      .Select(x => new ContractApplicationsDto
      {
        ApplicationId = x.ca.Id,
        ContractId = contractId,
        CandidateId = x.ca.UserId,
        FirstName = x.u.PentesterProfile!.FirstName,
        LastName = x.u.PentesterProfile.LastName,
        NickName = x.u.PentesterProfile.NickName,
        Bio = x.u.PentesterProfile.Bio,
        Status = x.ca.Status,
        AppliedAt = x.ca.AppliedAt,
      })
      .ToListAsync(ct);
  }

  private static string EscapedLike(string search)
  {
    return search
      .Replace(@"\", @"\\")
      .Replace("%", @"\%")
      .Replace("_", @"\_");
  }

  private static IQueryable<Contract> ApplySearch(
    IQueryable<Contract> query,
    string? search)
  {
    if (string.IsNullOrWhiteSpace(search))
      return query;

    var escaped = EscapedLike(search);

    return query.Where(c =>
        EF.Functions.ILike(c.Title, $"%{escaped}%") ||
        EF.Functions.ILike(c.Description, $"%{escaped}%"));
  }
}