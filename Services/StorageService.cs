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

        // Get the latest storage record for each department
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

        public async Task<Dictionary<string, decimal>> GetDepartmentAverageStorageAsync(int days = 30)
        {
            // Get the latest storage amount for each department
            var latestUsages = await GetLatestDepartmentUsageAsync();
            return latestUsages
                .GroupBy(s => s.Department.Name)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(s => s.StorageGB)
                );
        }

        public async Task<Dictionary<string, decimal>> GetDepartmentStorageChangePercentageAsync(int days = 30)
        {
            // Get all storage records within the time period
            var cutoffDate = DateTime.Now.AddDays(-days);
            var allRecords = await _context.StorageUsages
                .Include(s => s.Department)
                .Where(s => s.RecordDate >= cutoffDate)
                .OrderBy(s => s.RecordDate)
                .ToListAsync();

            // Group by department and calculate percentage change
            var changes = new Dictionary<string, decimal>();

            // Group records by department
            var departmentGroups = allRecords.GroupBy(s => s.Department.Name);

            foreach (var group in departmentGroups)
            {
                var departmentName = group.Key;
                var recordsByDate = group.OrderBy(s => s.RecordDate).ToList();

                // Need at least two records to calculate change
                if (recordsByDate.Count >= 2)
                {
                    var oldestRecord = recordsByDate.First();
                    var newestRecord = recordsByDate.Last();

                    // Calculate percentage change
                    decimal percentageChange = 0;
                    if (oldestRecord.StorageGB > 0)
                    {
                        percentageChange = ((newestRecord.StorageGB - oldestRecord.StorageGB) / oldestRecord.StorageGB) * 100;
                    }

                    changes[departmentName] = percentageChange;
                }
                else if (recordsByDate.Count == 1)
                {
                    // If there's only one record, we can't calculate a change, so we'll show 0%
                    changes[departmentName] = 0;
                }
            }

            return changes;
        }
    }
}