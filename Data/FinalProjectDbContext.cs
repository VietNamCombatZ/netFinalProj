using Microsoft.EntityFrameworkCore;
using finalProject.Models;


namespace finalProject.Data;

public class FinalProjectDbContext : DbContext
{
    public FinalProjectDbContext(DbContextOptions<FinalProjectDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>(); // Lưu enum thành chuỗi

        builder.Entity<Article>()
            .Property(a => a.Status)
            .HasConversion<string>();
        
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Article> Articles { get; set; }
}