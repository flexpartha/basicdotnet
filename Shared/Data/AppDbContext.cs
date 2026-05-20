using Microsoft.EntityFrameworkCore;
using UserApi.Features.Users;

namespace UserApi.Shared.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().OwnsOne(u => u.Address, a => a.OwnsOne(x => x.Geo));
        modelBuilder.Entity<User>().OwnsOne(u => u.Company);
    }
}

public class RevokedToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime RevokedAt { get; set; } = DateTime.UtcNow;
}
