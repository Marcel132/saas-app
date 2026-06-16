using backend.Api.Controllers;
using backend.Api.Controllers.Contracts.DTOs;
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

  public async Task<PagedResponse<ContractResponseDto>> GetContractsAsync(Guid userId, int page, int pageSize, string? search)
  {
    var query = _context.Contracts
      .AsNoTracking()
      .Where(c => c.ContractStatus == ContractStatus.Open && c.Deadline > DateTime.UtcNow && c.AuthorId != userId);

    if (!string.IsNullOrEmpty(search))
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
        HasApplied = _context.ContractApplications.Any(ca =>
        ca.ContractId == c.ContractId &&
        ca.CandidateId == userId)
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
      c.ContractId == contractId &&
      c.ContractStatus == ContractStatus.Open &&
      c.Deadline > DateTime.UtcNow
      )
      .Select(c => new ContractResponseDto
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
        HasApplied = _context.ContractApplications.Any(ca =>
          ca.ContractId == c.ContractId &&
          ca.CandidateId == userId
        )
      })
      .FirstOrDefaultAsync();

    return contract;
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
}