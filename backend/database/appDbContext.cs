using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UsersModel> users { get; set; }
    public DbSet<UserDataModel> user_data { get; set; }
    public DbSet<SessionModel> sessions { get; set; }
    public DbSet<ApiLogsModel> api_logs { get; set; }
    public DbSet<OpinionModel> opinions { get; set; }
    public DbSet<ContractModel> contracts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UsersModel>()
            .Property(u => u.Role)
            .HasConversion<string>();

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UsersModel>()
        .HasMany(u => u.Sessions)
        .WithOne(s => s.User)
        .HasForeignKey(s => s.UserId);

        modelBuilder.Entity<UsersModel>()
            .HasOne(u => u.UserData)
            .WithOne(ud => ud.User)
            .HasForeignKey<UserDataModel>(ud => ud.UserId);

        modelBuilder.Entity<UsersModel>()
            .HasMany(u => u.Opinions)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.TargetId);

        modelBuilder.Entity<UsersModel>()
            .HasMany(u => u.ApiLogs)
            .WithOne(l => l.User)
            .HasForeignKey(l => l.UserId);

        modelBuilder.Entity<UsersModel>()
            .HasMany(u => u.Contracts)
            .WithOne(c => c.Author)
            .HasForeignKey(c => c.AuthorId);

        modelBuilder.Entity<SessionModel>().Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<SessionModel>().Property(x => x.ExpiresAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP + interval '7 days'");
    }
        
    
}