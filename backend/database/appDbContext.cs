using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UsersModel> Users { get; set; }
    public DbSet<UserDataModel> UserData { get; set; }
    public DbSet<SessionsModel> Sessions { get; set; }
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

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity?.GetTableName()?.ToLower());
        }

        modelBuilder.Entity<UsersModel>()
            .Property(u => u.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<UsersModel>()
            .Property(u => u.SpecializationType)
            .HasColumnType("text[]");

        modelBuilder.Entity<UsersModel>()
            .HasOne(u => u.UserData)
            .WithOne(ud => ud.User)
            .HasForeignKey<UserDataModel>(ud => ud.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SessionsModel>().Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<SessionsModel>().Property(x => x.ExpiresAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP + interval '7 days'");

        modelBuilder.Entity<UsersModel>()
            .HasKey(u => u.Id);
            
        modelBuilder.Entity<UserDataModel>()
            .HasKey(u => u.UserId);
            
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
            
        modelBuilder.Entity<SessionsModel>()
            .HasKey(u => u.Id);
            
        modelBuilder.Entity<UserPermissionsModel>()
            .HasKey(u => new { u.UserId, u.PermissionId});
            
        modelBuilder.Entity<UserRolesModel>()
            .HasKey(u => new { u.UserId, u.RoleId});


    }
        
    
}