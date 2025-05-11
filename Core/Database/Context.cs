using Microsoft.EntityFrameworkCore;

namespace Core.Database;

public class Context : DbContext
{
    public DbSet<Models.Session> Sessions { get; set; }
    public DbSet<Models.Application> Applications { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseNpgsql("Host=localhost;Database=monkeyspeak;Username=postgres;Password=local");
}