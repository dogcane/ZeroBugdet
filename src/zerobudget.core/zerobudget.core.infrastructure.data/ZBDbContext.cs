using Microsoft.EntityFrameworkCore;
using zerobudget.core.domain;

namespace zerobudget.core.infrastructure.data;

public class ZBDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Bucket> Buckets { get; set; } = null!;
    public DbSet<MonthlyBucket> MonthlyBuckets { get; set; } = null!;
    public DbSet<Spending> Spendings { get; set; } = null!;
    public DbSet<Spending> MonthlySpendings { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Add entity configurations here if needed
        base.OnModelCreating(modelBuilder);
    }
}
