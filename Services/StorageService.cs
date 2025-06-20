using AuHackathon2025.Models;
using AuHackathon2025.Data;
using Microsoft.EntityFrameworkCore;

namespace AuHackathon2025.Services
{
    public class StorageService
    {
        private readonly AppDbContext _context;

        public StorageService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Department>> GetDepartmentsAsync()
        {
            return await _context.Departments.ToListAsync();
        }

        public async Task<StorageUsage> AddStorageUsageAsync(StorageUsage usage)
        {
            _context.StorageUsages.Add(usage);
            await _context.SaveChangesAsync();
            return usage;
        }

        public async Task<List<StorageUsage>> GetRecentUsageAsync(int days = 30)
        {
            var cutoffDate = DateTime.Now.AddDays(-days);
            return await _context.StorageUsages
                .Include(s => s.Department)
                .Where(s => s.RecordDate >= cutoffDate)
                .OrderBy(s => s.RecordDate)
                .ToListAsync();
        }

        // New method to get the latest storage record for each department
        public async Task<List<StorageUsage>> GetLatestDepartmentUsageAsync()
        {
            // Get a list of the most recent storage usage entry for each department
            var latestUsages = await _context.Departments
                .Select(d => new
                {
                    Department = d,
                    LatestUsage = d.StorageUsages
                        .OrderByDescending(s => s.RecordDate)
                        .FirstOrDefault()
                })
                .Where(x => x.LatestUsage != null)
                .Select(x => x.LatestUsage)
                .ToListAsync();

            return latestUsages;
        }

        public async Task<Dictionary<string, decimal>> GetDepartmentTotalCO2Async(int days = 30)
        {
            // Instead of summing up CO2 over time, get the latest CO2 for each department
            var latestUsages = await GetLatestDepartmentUsageAsync();
            return latestUsages
                .GroupBy(s => s.Department.Name)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(s => s.CO2Impact)
                );
        }

        public async Task<Dictionary<string, decimal>> GetDepartmentAverageStorageAsync(int days = 30)
        {
            // Instead of averaging, get the latest storage amount for each department
            var latestUsages = await GetLatestDepartmentUsageAsync();
            return latestUsages
                .GroupBy(s => s.Department.Name)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(s => s.StorageGB)
                );
        }
    }
}