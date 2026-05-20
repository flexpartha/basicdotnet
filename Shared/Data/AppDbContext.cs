using Microsoft.EntityFrameworkCore;
using UserApi.Features.Users;

namespace UserApi.Shared.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().OwnsOne(u => u.Address, a => a.OwnsOne(x => x.Geo));
        modelBuilder.Entity<User>().OwnsOne(u => u.Company);
        DbSeeder.Seed(modelBuilder);
    }
}
