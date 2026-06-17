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

  public async Task<PagedResponse<PublicContractDto>> GetPublicContractsAsync(QueryParams queryParams)
  {
    var query = _context.Contracts
      .AsNoTracking()
      .Where(c =>
      c.ContractStatus == ContractStatus.Open &&
      c.Deadline > DateTime.UtcNow
      );

    query = ApplySearch(query, queryParams.Search);

    var totalItems = await query.CountAsync();

    var contracts = await query
      .OrderByDescending(c => c.CreatedAt)
      .Skip((queryParams.Page - 1) * queryParams.PageSize)
      .Take(queryParams.PageSize)
      .Select(c => new PublicContractDto
      {
        ContractId = c.ContractId,
        Title = c.Title,
        Description = c.Description,
        ContractStatus = c.ContractStatus,
        CreatedAt = c.CreatedAt,
        Deadline = c.Deadline,
        Price = c.Price,
        UpdatedAt = c.UpdatedAt
      })
      .ToListAsync();

    return new PagedResponse<PublicContractDto>
    {
      Page = queryParams.Page,
      PageSize = queryParams.PageSize,
      TotalItems = totalItems,
      TotalPages = (int)Math.Ceiling(totalItems / (double)queryParams.PageSize),
      Items = contracts
    };
  }

  public async Task<PagedResponse<PentesterContractDto>> GetPentesterContractsAsync(Guid userId, QueryParams queryParams)
  {
    var query = _context.Contracts
      .AsNoTracking()
      .Where(c =>
        c.ContractStatus == ContractStatus.Open &&
        c.Deadline > DateTime.UtcNow
      );

    query = ApplySearch(query, queryParams.Search);

    var totalItems = await query.CountAsync();

    var contracts = await query
     .OrderByDescending(c => c.CreatedAt)
     .Skip((queryParams.Page - 1) * queryParams.PageSize)
     .Take(queryParams.PageSize)
     .Select(c => new PentesterContractDto
     {
       ContractId = c.ContractId,
       Title = c.Title,
       Description = c.Description,
       ContractStatus = c.ContractStatus,
       CreatedAt = c.CreatedAt,
       Deadline = c.Deadline,
       Price = c.Price,
       UpdatedAt = c.UpdatedAt,
       HasApplied = _context.ContractApplications
         .Any(ca =>
           ca.ContractId == c.ContractId &&
           ca.CandidateId == userId
         )
     })
     .ToListAsync();

    return new PagedResponse<PentesterContractDto>
    {
      Page = queryParams.Page,
      PageSize = queryParams.PageSize,
      TotalItems = totalItems,
      TotalPages = (int)Math.Ceiling(totalItems / (double)queryParams.PageSize),
      Items = contracts
    };
  }

  public async Task<PagedResponse<CompanyContractDto>> GetCompanyContractsAsync(Guid userId, QueryParams queryParams)
  {
    var query = _context.Contracts
      .AsNoTracking()
      .Where(c =>
        c.AuthorId == userId
      );

    query = ApplySearch(query, queryParams.Search);

    var totalItems = await query.CountAsync();

    var contracts = await query
      .OrderBy(c =>
        c.ContractStatus == ContractStatus.InProgress ? 0 :
        c.ContractStatus == ContractStatus.Open ? 1 :
        c.ContractStatus == ContractStatus.Completed ? 2 :
        3
      )
      .ThenByDescending(c => c.CreatedAt)
      .Skip((queryParams.Page - 1) * queryParams.PageSize)
      .Take(queryParams.PageSize)
      .Select(c => new CompanyContractDto
      {
        ContractId = c.ContractId,
        ContractStatus = c.ContractStatus,
        CreatedAt = c.CreatedAt,
        Deadline = c.Deadline,
        Description = c.Description,
        Price = c.Price,
        Title = c.Title,
        UpdatedAt = c.UpdatedAt
      })
      .ToListAsync();

    return new PagedResponse<CompanyContractDto>
    {
      Page = queryParams.Page,
      PageSize = queryParams.PageSize,
      TotalItems = totalItems,
      TotalPages = (int)Math.Ceiling(totalItems / (double)queryParams.PageSize),
      Items = contracts
    };
  }

  public async Task<ContractDetailsDto?> GetContractDetailsAsync(long contractId, Guid? userId)
  {
    var query = _context.Contracts
      .AsNoTracking()
      .Where(c =>
        c.ContractId == contractId
      );
    if (userId.HasValue)
    {
      query = query
        .Where(c =>
          c.AuthorId == userId ||
          (
            c.ContractStatus == ContractStatus.Open &&
            c.Deadline > DateTime.UtcNow
          )
        );
    }
    else
    {
      query = query.Where(c =>
          c.ContractStatus == ContractStatus.Open
      );
    }

    return await query
      .Select(c => new ContractDetailsDto
      {
        ContractId = c.ContractId,
        ContractStatus = c.ContractStatus,
        CreatedAt = c.CreatedAt,
        Deadline = c.Deadline,
        Description = c.Description,
        Price = c.Price,
        Title = c.Title,
        HasApplied = _context.ContractApplications
          .Any(ca =>
            ca.ContractId == c.ContractId &&
            ca.CandidateId == userId
          )
      })
      .FirstOrDefaultAsync();
  }

  public async Task<List<ContractApplicationsDto>> GetContractApplicationsAsync(long contractId)
  {
    return await _context.ContractApplications
      .AsNoTracking()
      .Where(ca => ca.ContractId == contractId)
      .Join(
        _context.Users,
        ca => ca.CandidateId,
        u => u.Id,
        (ca, u) => new ContractApplicationsDto
        {
          ApplicationId = ca.ApplicationId,
          ContractId = contractId,
          CandidateId = ca.CandidateId,
          FirstName = u.UserData.FirstName,
          LastName = u.UserData.LastName,
          Nickname = u.UserData.Nickname ?? string.Empty,
          Description = u.UserData.Description,
          Status = ca.Status,
          AppliedAt = ca.AppliedAt
        })
        .ToListAsync();
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