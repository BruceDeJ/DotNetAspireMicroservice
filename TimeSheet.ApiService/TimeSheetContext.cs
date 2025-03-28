using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using TimeSheet.ApiService.Domain;

namespace TimeSheet.ApiService
{
    public class TimeSheetContext : DbContext
    {
        public TimeSheetContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<TimeSheetEntry> TimeSheetEntries { get; set; }
    }
}
