using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  public DbSet<User> Users { get; set; }
  public DbSet<Session> Sessions { get; set; }
  public DbSet<Role> Roles { get; set; }
  public DbSet<UserRole> UserRoles { get; set; }
  public DbSet<Contract> Contracts { get; set; }
  public DbSet<ContractApplication> ContractApplications { get; set; }
  public DbSet<ContractAssignment> ContractAssignments { get; set; }
  public DbSet<ContractExecution> ContractExecutions { get; set; }
  public DbSet<Permission> Permissions { get; set; }
  public DbSet<RolePermission> RolePermissions { get; set; }
  public DbSet<UserPermission> UserPermissions { get; set; }


  // // AUTH
  // public DbSet<RolePermissionsModel> RolePermissions { get; set; }
  // public DbSet<UserPermissionsModel> UserPermissions { get; set; }
  // public DbSet<ApiLogsModel> ApiLogs { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>(entity =>
    {

      entity.OwnsOne(u => u.UserData, ud =>
        {
          ud.ToTable("user_data");
          
          ud.WithOwner()
            .HasForeignKey("user_id");

          ud.Property(ud => ud.FirstName)
            .HasColumnName("first_name");

          ud.Property(ud => ud.LastName)
            .HasColumnName("last_name");
          
          ud.Property(ud => ud.Nickname)
            .HasColumnName("nickname");

          ud.Property(ud => ud.PhoneNumber)
            .HasColumnName("phone_number");

          ud.Property(ud => ud.Skills)
            .HasColumnName("skills");

          ud.Property(ud => ud.City)
            .HasColumnName("city");

          ud.Property(ud => ud.Country)
            .HasColumnName("country");

          ud.Property(ud => ud.PostalCode)
            .HasColumnName("postal_code");

          ud.Property(ud => ud.Street)
            .HasColumnName("street");

          ud.Property(ud => ud.CompanyName)
            .HasColumnName("company_name");

          ud.Property(ud => ud.CompanyNip)
            .HasColumnName("company_nip");

          ud.Property(ud => ud.IsEmailVerified)
            .HasColumnName("is_email_verified");

          ud.Property(ud => ud.IsProfileCompleted)
              .HasColumnName("is_profile_completed");

          ud.Property(ud => ud.IsTwoFactorEnabled)
            .HasColumnName("is_two_factor_enabled");
        });

      entity.ToTable("users");

      entity.HasKey(u => u.Id);

      entity.Property(u => u.Id)
      .HasColumnName("user_id");

      entity.Property(u => u.Email)
        .HasColumnName("email")
        .IsRequired()
        .HasMaxLength(255);

      entity.Property(u => u.PasswordHash)
        .HasColumnName("password_hash");

      entity.Property(u => u.CreatedAt)
        .HasColumnName("created_at");
        
      entity.Property(u => u.FailedLoginAttempts)
        .HasColumnName("failed_login_attempts");

      entity.Property(u => u.LoginBlockedUntil)
        .HasColumnName("login_blocked_until");

      entity.Property(u => u.IsActive)
        .HasColumnName("is_active");

      entity.HasMany(s => s.UserSpecializations)
        .WithOne(us => us.User)
        .HasForeignKey(us => us.UserId)
        .OnDelete(DeleteBehavior.Cascade);

      entity.Navigation(u => u.UserSpecializations)
        .HasField("_userSpecializations")
        .UsePropertyAccessMode(PropertyAccessMode.Field);

      entity.HasMany(u => u.UserRoles)
        .WithOne(ur => ur.User)
        .HasForeignKey(ur => ur.UserId)
        .OnDelete(DeleteBehavior.Cascade);

      entity.Navigation(u => u.UserRoles)
        .HasField("_userRole")
        .UsePropertyAccessMode(PropertyAccessMode.Field);
    });

    modelBuilder.Entity<UserSpecialization>(entity =>
    {
      entity.ToTable("user_specializations");

      entity.HasKey(x => new { x.UserId, x.Specialization });

      entity.Property(x => x.UserId)
        .HasColumnName("user_id");

      entity.Property(x => x.Specialization)
        .HasColumnName("specialization");
    });
    
    modelBuilder.Entity<UserRole>(entity =>
    {
      entity.ToTable("user_roles");

      entity.HasKey(x => new {x.UserId, x.RoleId});

      entity.Property(x => x.UserId)
        .HasColumnName("user_id");
      
      entity.Property(x => x.RoleId)
        .HasColumnName("role_id");
      
      entity.Property(x => x.AssignedAt)
        .HasColumnName("assigned_at");
      
    });

    modelBuilder.Entity<Session>(entity =>
    {
      entity.HasKey(s => s.SessionId);

      entity.ToTable("sessions");

      entity.Property(s => s.SessionId)
        .HasColumnName("id");

      entity.Property(s => s.CreatedAt)
        .HasColumnName("created_at");

      entity.Property(s => s.ExpiresAt)
        .HasColumnName("expires_at");

      entity.Property(s => s.IpAddress)
        .HasColumnName("ip");

      entity.Property(s => s.UserAgent)
        .HasColumnName("user_agent");

      entity.Property(s => s.RefreshTokenHash)
        .HasColumnName("refresh_token");

      entity.Property(s => s.Revoked)
        .HasColumnName("revoked");

      entity.Property(s => s.UserId)
        .HasColumnName("user_id");
      
      entity.Property(s => s.Used)
        .HasColumnName("used");

      entity.Property(s => s.ReplacedByTokenId)
        .HasColumnName("replaced_by_token_id");
    });

    modelBuilder.Entity<Role>(entity =>
    {
      entity.ToTable("roles");

      entity.HasKey(r => r.RoleId);

      entity.Property(r => r.RoleId)
        .HasColumnName("role_id");

      entity.Property(r => r.Code)
        .HasColumnName("code");

      entity.Property(r => r.Name)
        .HasColumnName("name");

      entity.Property(r => r.IsActive)
        .HasColumnName("is_active");
    });
    
    modelBuilder.Entity<Contract>(entity =>
    {
      entity.ToTable("contracts");
      entity.HasKey(c => c.ContractId);

      entity.Property(c => c.ContractId)
        .HasColumnName("contract_id");
      
      entity.Property(c => c.AuthorId)
        .HasColumnName("author_id");
      
      entity.Property(c => c.Title)
        .HasColumnName("title");
      
      entity.Property(c => c.Description)
        .HasColumnName("description");
      
      entity.Property(c => c.Price)
        .HasColumnName("price");

      entity.Property(c => c.ContractStatus)
        .HasColumnName("contract_status");
      
      entity.Property(c => c.CreatedAt)
        .HasColumnName("created_at");
      
      entity.Property(c => c.UpdatedAt)
        .HasColumnName("updated_at");

      entity.Property(c => c.Deadline)
        .HasColumnName("deadline");

    });
    modelBuilder.HasPostgresEnum<ContractStatus>("contract_status");

    modelBuilder.Entity<ContractApplication>(entity =>
    {
      entity.ToTable("contract_applications");
      entity.HasKey(ca => ca.ApplicationId);

      entity.Property(ca => ca.ApplicationId)
        .HasColumnName("application_id");
      
      entity.Property(ca => ca.ContractId)
        .HasColumnName("contract_id");
      
      entity.Property(ca => ca.CandidateId)
        .HasColumnName("candidate_id");
      
      entity.Property(ca => ca.Status)
        .HasColumnName("applied_status");

      entity.Property(ca => ca.AppliedAt)
        .HasColumnName("applied_at");
    });
    modelBuilder.HasPostgresEnum<ContractApplicationStatus>("contract_application_status");

    modelBuilder.Entity<ContractAssignment>(entity =>
    {
      entity.ToTable("contract_assignments");
      entity.HasKey(ca => ca.AssignmentId);

      entity.Property(ca => ca.AssignmentId)
        .HasColumnName("assignment_id");
      
      entity.Property(ca => ca.ContractId)
        .HasColumnName("contract_id");
      
      entity.Property(ca => ca.DeveloperId)
        .HasColumnName("user_id");
      
      entity.Property(ca => ca.AssignedAt)
        .HasColumnName("assigned_at");
    });

    modelBuilder.Entity<ContractExecution>(entity =>
    {
      entity.ToTable("contract_executions");
      entity.HasKey(ce => ce.ExecutionId);

      entity.Property(ce => ce.ExecutionId)
        .HasColumnName("execution_id");
      
      entity.Property(ce => ce.ContractId)
        .HasColumnName("contract_id");

      entity.Property(ce => ce.StartedAt)
        .HasColumnName("started_at");

      entity.Property(ce => ce.CompletedAt)
        .HasColumnName("completed_at");
      
      entity.Property(ce => ce.Status)
        .HasColumnName("execution_status");
        
      entity.Property(ce => ce.ReportUrl)
        .HasColumnName("report_url");
    });
    modelBuilder.HasPostgresEnum<ContractExecutionStatus>("contract_execution_status");

    modelBuilder.Entity<Permission>(entity =>
    {
      entity.ToTable("permissions");
      entity.HasKey(p => p.PermissionId);

      entity.Property(p => p.PermissionId)
        .HasColumnName("permission_id");
      entity.Property(p => p.Action)
        .HasColumnName("action");
      entity.Property(p => p.Resource)
        .HasColumnName("resource");
      entity.Property(p => p.Code)
        .HasColumnName("code");
      entity.Property(p => p.Description)
        .HasColumnName("description");
      entity.Property(p => p.IsActive)
        .HasColumnName("is_active");
      entity.Property(p => p.CreatedAt)
        .HasColumnName("created_at");
      
      entity.HasIndex(p => p.Code)
        .IsUnique();
    });
    modelBuilder.Entity<RolePermission>(entity =>
    {
      entity.ToTable("role_permissions");
      entity.HasKey(rp => new {rp.RoleId, rp.PermissionId});

      entity.Property(rp => rp.RoleId)
        .HasColumnName("role_id");
      
      entity.Property(rp => rp.PermissionId)
        .HasColumnName("permission_id");
    });
    modelBuilder.Entity<UserPermission>(entity =>
    {
      entity.ToTable("user_permissions");
      entity.HasKey(up => new {up.UserId, up.PermissionId});

      entity.Property(up => up.UserId)
        .HasColumnName("user_id");

      entity.Property(up => up.PermissionId)
        .HasColumnName("permission_id");

      entity.Property(up => up.IsDenied)
        .HasColumnName("is_denied");

      entity.Property(up => up.AssignedAt)
        .HasColumnName("granted_at");
    });
    
    // modelBuilder.Entity<ApiLogsModel>().ToTable("api_logs");



    // modelBuilder.Entity<ApiLogsModel>()
    //     .HasKey(u => u.Id);

    // modelBuilder.Entity<PermissionsModel>()
    //     .HasKey(u => u.PermissionId);

    // modelBuilder.Entity<RolePermissionsModel>()
    //     .HasKey(u => new { u.PermissionId, u.RoleId });


    // modelBuilder.Entity<UserPermissionsModel>()
    //     .HasKey(u => new { u.UserId, u.PermissionId });
  }


}