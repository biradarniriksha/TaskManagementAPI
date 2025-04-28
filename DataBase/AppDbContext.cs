using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.DataBase
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed Users with hashed passwords
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin123",
                    Role = "Admin"
                },
                new User
                {
                    Id = 2,
                    Username = "user1",
                    Password = "user123",
                    Role = "User"
                }
            );

            // Seed sample tasks
            modelBuilder.Entity<TaskItem>().HasData(
                new TaskItem
                {
                    Id = 1,
                    Title = "Complete API Documentation",
                    Description = "Write all API documentation for the project",
                    UserId = 1
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Implement Authentication",
                    Description = "Finish JWT authentication implementation",
                    UserId = 2
                }
            );
        }
    }
}
