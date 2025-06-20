using System.ComponentModel.DataAnnotations;

namespace AuHackathon2025.Models;

public class StorageUsage
{
    public int Id { get; set; }

    public int DepartmentId { get; set; }

    [Required]
    public DateTime RecordDate { get; set; }

    // Storage in GB
    [Required]
    public decimal StorageGB { get; set; }

    // Calculated CO2 impact (kg) - using fixed factor
    public decimal CO2Impact => StorageGB * CO2_FACTOR_PER_GB;

    // CO2 per GB of storage (kg) - industry average
    private const decimal CO2_FACTOR_PER_GB = 0.5m;

    // Annual CO2 emissions (kg) - 1.62g CO₂ per GB per year converted to kg
    public decimal AnnualCO2Impact => StorageGB * ANNUAL_CO2_FACTOR_PER_GB;

    // CO2 per GB per year (kg) - 1.62g converted to kg
    private const decimal ANNUAL_CO2_FACTOR_PER_GB = 0.00162m;

    public string? Notes { get; set; }

    // Navigation property
    public Department Department { get; set; } = null!;
}