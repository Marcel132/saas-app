using backend.Api.Controllers.Users.DTOs;
using backend.Domain.Entities.Enum;
using backend.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Infrastructure.Persistence.Repositories;

public class UserQueryRepository : IUserQueryRepository
{
  private readonly AppDbContext _context;
  public UserQueryRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task<RoleType> GetRoleTypeAsync(Guid userId)
  {
    var user = await _context.Users
      .AsNoTracking()
      .Where(u => u.Id == userId && u.IsActive)
      .Select(u => new { u.RoleType })
      .FirstOrDefaultAsync()
      ?? throw new NotFoundAppException();

    return user.RoleType;
  }

  public async Task<UserPublicPentesterDto> GetPentesterByIdAsync(Guid userId, CancellationToken ct)
  {
    return await _context.Users
      .AsNoTracking()
      .Where(u => u.Id == userId && u.IsActive && u.PentesterProfile != null)
      .Select(u => new UserPublicPentesterDto
      {
        Id = u.Id,
        FirstName = u.PentesterProfile!.FirstName,
        LastName = u.PentesterProfile.LastName,
        NickName = u.PentesterProfile.NickName,
        Bio = u.PentesterProfile.Bio,
        GithubUrl = u.PentesterProfile.GithubUrl,
        LinkedinUrl = u.PentesterProfile.LinkedinUrl,
        Experience = u.PentesterProfile.Experience,
        Certificates = u.PentesterProfile.Certificates
          .Select(c => c.Certificate)
          .ToList(),
        Specializations = u.PentesterProfile.Specializations
          .Select(s => s.Specialization)
          .ToList(),
        CreatedAt = u.CreatedAt
      })
      .FirstOrDefaultAsync(ct)
      ?? throw new NotFoundAppException();
  }

  public async Task<PentesterPrivateDto> GetCurrentPentesterAsync(Guid userId)
  {
    return await _context.Users
      .AsNoTracking()
      .Where(u => u.Id == userId && u.IsActive && u.PentesterProfile != null)
      .Select(u => new PentesterPrivateDto
      {
        Id = u.Id,
        Email = u.Email,
        Role = u.RoleType,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt,

        FirstName = u.PentesterProfile!.FirstName,
        LastName = u.PentesterProfile.LastName,
        NickName = u.PentesterProfile.NickName,
        Phone = u.PentesterProfile.Phone,
        Country = u.PentesterProfile.Country,
        City = u.PentesterProfile.City,
        Street = u.PentesterProfile.Street,
        PostalCode = u.PentesterProfile.PostalCode,
        Bio = u.PentesterProfile.Bio,
        GithubUrl = u.PentesterProfile.GithubUrl,
        LinkedinUrl = u.PentesterProfile.LinkedinUrl,
        Experience = u.PentesterProfile.Experience,
        Certificates = u.PentesterProfile.Certificates
          .Select(c => c.Certificate)
          .ToList(),
        Specializations = u.PentesterProfile.Specializations
          .Select(s => s.Specialization)
          .ToList(),

        Roles = u.UserRoles
          .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
          .ToHashSet(),
        Permissions = u.UserRoles
          .Select(ur => ur.RoleId)
          .Join(_context.RolePermissions, roleId => roleId, rp => rp.RoleId, (roleId, rp) => rp.PermissionId)
          .Join(_context.Permissions, permId => permId, p => p.Id, (permId, p) => p.Code)
          .ToHashSet()
      })
      .FirstOrDefaultAsync()
      ?? throw new NotFoundAppException();
  }

  public async Task<CompanyPrivateDto> GetCurrentCompanyAsync(Guid userId)
  {
    return await _context.Users
      .AsNoTracking()
      .Where(u => u.Id == userId && u.IsActive && u.CompanyProfile != null)
      .Select(u => new CompanyPrivateDto
      {
        Id = u.Id,
        Email = u.Email,
        Role = u.RoleType,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt,

        Nip = u.CompanyProfile!.Nip,
        Name = u.CompanyProfile.Name,
        Phone = u.CompanyProfile.Phone,
        Country = u.CompanyProfile.Country,
        City = u.CompanyProfile.City,
        Street = u.CompanyProfile.Street,
        PostalCode = u.CompanyProfile.PostalCode,
        Bio = u.CompanyProfile.Bio,
        WebsiteUrl = u.CompanyProfile.WebsiteUrl,

        Roles = u.UserRoles
          .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
          .ToHashSet(),
        Permissions = u.UserRoles
          .Select(ur => ur.RoleId)
          .Join(_context.RolePermissions, roleId => roleId, rp => rp.RoleId, (roleId, rp) => rp.PermissionId)
          .Join(_context.Permissions, permId => permId, p => p.Id, (permId, p) => p.Code)
          .ToHashSet()
      })
      .FirstOrDefaultAsync()
      ?? throw new NotFoundAppException();
  }

  public async Task<List<UserContractsDto>> GetCurrentUserContractsAsync(Guid userId, ContractStatus? status, CancellationToken ct)
  {
    var query = _context.Contracts
      .AsNoTracking()
      .Where(c =>
        c.AuthorId == userId ||
        _context.ContractAssignments.Any(ca =>
          ca.ContractId == c.Id &&
          ca.PentesterId == userId
        )
      );

    if (status.HasValue)
      query = query.Where(c => c.Status == status.Value);

    return await query
      .Select(c => new UserContractsDto
      {
        ContractId = c.Id,
        Title = c.Title,
        Description = c.Description,
        ContractStatus = c.Status,
        CreatedAt = c.CreatedAt
      })
      .ToListAsync(ct);
  }

  public async Task<List<UserApplicationsDto>> GetApplicationsAsync(Guid userId, ContractApplicationStatus? status, CancellationToken ct)
  {
    var query = _context.ContractApplications
      .AsNoTracking()
      .Where(ca => ca.UserId == userId)
      .Join(
        _context.Contracts,
        ca => ca.ContractId,
        c => c.Id,
        (ca, c) => new UserApplicationsDto
        {
          ApplicationId = ca.Id,
          ContractId = ca.ContractId,
          CompanyId = c.AuthorId,
          Status = ca.Status,
          AppliedAt = ca.AppliedAt
        }
      );

    if (status.HasValue)
      query = query.Where(ca => ca.Status == status.Value);

    return await query.ToListAsync(ct);
  }

  public async Task<UserSummaryDto> GetSummary(Guid userId, CancellationToken ct)
  {
    var activeTask = await _context.ContractAssignments
      .AsNoTracking()
      .CountAsync(ca =>
        ca.PentesterId == userId &&
        ca.Contract.Status == ContractStatus.InProgress,
        ct
      );

    var activeOrders = await _context.Contracts
      .AsNoTracking()
      .CountAsync(c =>
        c.AuthorId == userId &&
        (
          c.Status == ContractStatus.Open ||
          c.Status == ContractStatus.InProgress
        ),
        ct
      );

    var completedReports = await _context.ContractReports
      .AsNoTracking()
      .CountAsync(cr =>
        cr.Status == ContractReportStatus.Approved &&
        (
          cr.ContractAssignment.PentesterId == userId ||
          cr.ContractAssignment.Contract.AuthorId == userId
        ),
        ct
      );

    var totalReports = await _context.ContractReports
      .AsNoTracking()
      .CountAsync(cr =>
        cr.ContractAssignment.PentesterId == userId ||
        cr.ContractAssignment.Contract.AuthorId == userId,
        ct
      );

    return new UserSummaryDto()
    {
      ActiveTasks = activeTask,
      ActiveOrders = activeOrders,
      CompletedReports = completedReports,
      TotalReports = totalReports
    };
  }
}