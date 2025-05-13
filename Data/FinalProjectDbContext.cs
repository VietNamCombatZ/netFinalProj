using Microsoft.EntityFrameworkCore;
using finalProject.Models;
using finalProject.Models.Enums;


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
        
        // builder.Entity<User>().HasData(
        //     new User
        //     {
        //         UserId = 1001,
        //         Name = "Nguyễn Văn Quản Trị",
        //         DateofBirth = new DateTime(1990, 1, 1),
        //         Email = "admin@sciencehub.com",
        //         Username = "admin01",
        //         Password = "admin123", // 🔐 chỉ dùng cho demo, không nên dùng plaintext
        //         Role = Role.Admin
        //     }
        // );
        
        builder.Entity<Topic>().HasData(
            new Topic { TopicId = 1, TopicName = "Trí tuệ nhân tạo" },
            new Topic { TopicId = 2, TopicName = "Khoa học dữ liệu" },
            new Topic { TopicId = 3, TopicName = "Mạng máy tính" },
            new Topic { TopicId = 4, TopicName = "An toàn thông tin" },
            new Topic { TopicId = 5, TopicName = "Công nghệ phần mềm" }
        );
        
        
        
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Article> Articles { get; set; }
    
    public DbSet<Topic> Topics { get; set; }
}