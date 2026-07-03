using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
  public DbSet<User> Users => Set<User>();

  public DbSet<CompanyProfile> CompanyProfiles => Set<CompanyProfile>();
  public DbSet<PentesterProfile> PentesterProfiles => Set<PentesterProfile>();
  public DbSet<PentesterCertificate> PentesterCertificates => Set<PentesterCertificate>();
  public DbSet<PentesterSpecialization> PentesterSpecializations => Set<PentesterSpecialization>();

  public DbSet<Role> Roles => Set<Role>();
  public DbSet<Permission> Permissions => Set<Permission>();

  public DbSet<UserRole> UserRoles => Set<UserRole>();
  public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
  public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

  public DbSet<Session> Sessions => Set<Session>();

  public DbSet<Contract> Contracts => Set<Contract>();
  public DbSet<ContractApplication> ContractApplications => Set<ContractApplication>();
  public DbSet<ContractAssignment> ContractAssignments => Set<ContractAssignment>();
  public DbSet<ContractRequest> ContractRequests => Set<ContractRequest>();
  public DbSet<ContractReport> ContractReports => Set<ContractReport>();
  public DbSet<ContractVulnerability> ContractVulnerabilities => Set<ContractVulnerability>();

  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(
        typeof(AppDbContext).Assembly
    );
  }
}