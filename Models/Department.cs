using System.ComponentModel.DataAnnotations;

namespace AuHackathon2025.Models;

public class Department
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public int EmployeeCount { get; set; }

    // Navigation property
    public List<StorageUsage> StorageUsages { get; set; } = new();
}