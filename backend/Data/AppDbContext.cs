using Microsoft.EntityFrameworkCore;
using HelloWorldFunctionApp.Models;

namespace HelloWorldFunctionApp.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToContainer("Users");
        modelBuilder.Entity<User>().HasKey(u => u.Id);
    }
}

