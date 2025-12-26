using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UsersModel> Users { get; set; }
    public DbSet<UserDataModel> UserData { get; set; }
    public DbSet<SessionModel> Sessions { get; set; }
    public DbSet<ApiLogsModel> ApiLogs { get; set; }
    public DbSet<OpinionModel> Opinions { get; set; }
    public DbSet<ContractModel> Contracts { get; set; }
    public DbSet<ContractApplicationModel> ContractApplications { get; set; }

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


        // modelBuilder.Entity<UsersModel>()
        // .HasMany(u => u.Sessions)
        // .WithOne(s => s.User)
        // .HasForeignKey(s => s.UserId);

        // modelBuilder.Entity<UsersModel>()
        //     .HasOne(u => u.UserData)
        //     .WithOne(ud => ud.User)
        //     .HasForeignKey<UserDataModel>(ud => ud.UserId)
        //     .IsRequired();

        // modelBuilder.Entity<UsersModel>()
        //     .HasMany(u => u.Opinions)
        //     .WithOne(o => o.User);

        // modelBuilder.Entity<UsersModel>()
        //     .HasMany(u => u.ApiLogs)
        //     .WithOne(l => l.User)
        //     .HasForeignKey(l => l.UserId);

        // modelBuilder.Entity<UsersModel>()
        //     .HasMany(u => u.Contracts)
        //     .WithOne(c => c.Author)
        //     .HasForeignKey(c => c.AuthorId);
        // modelBuilder.Entity<UsersModel>()
        //     .HasMany(u => u.ContractsApplication)
        //     .WithOne(ca => ca.User)
        //     .HasForeignKey(ca => ca.UserId);

        modelBuilder.Entity<SessionModel>().Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<SessionModel>().Property(x => x.ExpiresAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP + interval '7 days'");

        modelBuilder.Entity<ApiLogsModel>().ToTable("api_logs");
        modelBuilder.Entity<UserDataModel>().ToTable("user_data");
        modelBuilder.Entity<ContractApplicationModel>().ToTable("contract_applications");


    }
        
    
}