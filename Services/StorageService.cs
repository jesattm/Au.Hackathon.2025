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

        public async Task<Dictionary<string, decimal>> GetDepartmentTotalCO2Async(int days = 30)
        {
            var cutoffDate = DateTime.Now.AddDays(-days);
            return await _context.StorageUsages
                .Include(s => s.Department)
                .Where(s => s.RecordDate >= cutoffDate)
                .GroupBy(s => s.Department.Name)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.Sum(s => s.CO2Impact)
                );
        }

        public async Task<Dictionary<string, decimal>> GetDepartmentAverageStorageAsync(int days = 30)
        {
            var cutoffDate = DateTime.Now.AddDays(-days);
            return await _context.StorageUsages
                .Include(s => s.Department)
                .Where(s => s.RecordDate >= cutoffDate)
                .GroupBy(s => s.Department.Name)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.Average(s => s.StorageGB)
                );
        }
    }
}