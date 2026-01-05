using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  public DbSet<User> Users { get; set; }
  // public DbSet<UserData> UserData { get; set; }
  public DbSet<Session> Sessions { get; set; }



  public DbSet<ApiLogsModel> ApiLogs { get; set; }
  public DbSet<OpinionsModel> Opinions { get; set; }
  public DbSet<ContractsModel> Contracts { get; set; }
  public DbSet<ContractApplicationModel> ContractApplications { get; set; }


  // AUTH
  public DbSet<PermissionsModel> Permissions { get; set; }
  public DbSet<RolePermissionsModel> RolePermissions { get; set; }
  public DbSet<RolesModel> Roles { get; set; }
  public DbSet<UserRolesModel> UserRoles { get; set; }
  public DbSet<UserPermissionsModel> UserPermissions { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>(entity =>
    {
      // entity.HasOne(u => u.UserData)
      //   .WithOne()
      //   .HasForeignKey<UserData>(ud => ud.UserId)
      //   .OnDelete(DeleteBehavior.Cascade);

      entity.OwnsOne(u => u.UserData, ud =>
        {
          ud.ToTable("user_data");
          
          ud.WithOwner()
            .HasForeignKey("user_id");

          ud.Property(p => p.FirstName)
            .HasColumnName("first_name");

          ud.Property(ud => ud.LastName)
            .HasColumnName("last_name");

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
        
      entity.Property(u => u.Specializations)
        .HasColumnName("specialization")
        .HasColumnType("text[]")
        .IsRequired();

      entity.Property(u => u.FailedLoginAttempts)
        .HasColumnName("failed_login_attempts");

      entity.Property(u => u.LoginBlockedUntil)
        .HasColumnName("login_blocked_until");

      entity.Property(u => u.IsActive)
        .HasColumnName("is_active");
    });

    modelBuilder.Entity<Session>(entity =>
    {
      entity.HasKey(s => s.SessionId);

      entity.ToTable("sessions");

      entity.HasKey(s => s.SessionId);

      entity.Property(s => s.SessionId)
        .HasColumnName("id");

      entity.Property(s => s.CreatedAt)
        .HasColumnName("created_at");

      entity.Property(s => s.ExpiresAt)
        .HasColumnName("expires_at");

      entity.Property(s => s.Ip)
        .HasColumnName("ip");

      entity.Property(s => s.UserAgent)
        .HasColumnName("user_agent");

      entity.Property(s => s.RefreshTokenHash)
        .HasColumnName("refresh_token");

      entity.Property(s => s.Revoked)
        .HasColumnName("revoked");

      entity.Property(s => s.UserId)
        .HasColumnName("user_id");
    });

    modelBuilder.Entity<User>().ToTable("users");
    modelBuilder.Entity<Session>().ToTable("sessions");

    modelBuilder.Entity<ApiLogsModel>().ToTable("api_logs");
    modelBuilder.Entity<ContractsModel>().ToTable("contracts");

    modelBuilder.Entity<ApiLogsModel>()
        .HasKey(u => u.Id);

    modelBuilder.Entity<ContractApplicationModel>()
        .HasKey(u => u.Id);

    modelBuilder.Entity<ContractsModel>()
        .HasKey(u => u.Id);

    modelBuilder.Entity<OpinionsModel>()
        .HasKey(u => u.Id);

    modelBuilder.Entity<PermissionsModel>()
        .HasKey(u => u.PermissionId);

    modelBuilder.Entity<RolePermissionsModel>()
        .HasKey(u => new { u.PermissionId, u.RoleId });

    modelBuilder.Entity<RolesModel>()
        .HasKey(u => u.RoleId);


    modelBuilder.Entity<UserPermissionsModel>()
        .HasKey(u => new { u.UserId, u.PermissionId });

    modelBuilder.Entity<UserRolesModel>()
        .HasKey(u => new { u.UserId, u.RoleId });
  }


}