using AuHackathon2025.Models;
using Microsoft.EntityFrameworkCore;

namespace AuHackathon2025.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Department> Departments { get; set; }
    public DbSet<StorageUsage> StorageUsages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure relationships
        modelBuilder.Entity<StorageUsage>()
            .HasOne(s => s.Department)
            .WithMany(d => d.StorageUsages)
            .HasForeignKey(s => s.DepartmentId);

        // Seed departments
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "Architecture", EmployeeCount = 12 },
            new Department { Id = 2, Name = "Administration", EmployeeCount = 8 },
            new Department { Id = 3, Name = "Project Management", EmployeeCount = 15 },
            new Department { Id = 4, Name = "Development", EmployeeCount = 25 }
        );
    }
}
