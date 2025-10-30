using Microsoft.EntityFrameworkCore;
using zerobudget.core.domain;
using zerobudget.core.infrastructure.data.Configurations;

namespace zerobudget.core.infrastructure.data;

public class ZBDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Bucket> Buckets { get; set; } = null!;
    public DbSet<MonthlyBucket> MonthlyBuckets { get; set; } = null!;
    public DbSet<Spending> Spendings { get; set; } = null!;
    public DbSet<MonthlySpending> MonthlySpendings { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all entity configurations
        modelBuilder.ApplyConfiguration(new BucketConfiguration());
        modelBuilder.ApplyConfiguration(new MonthlyBucketConfiguration());
        modelBuilder.ApplyConfiguration(new SpendingConfiguration());
        modelBuilder.ApplyConfiguration(new MonthlySpendingConfiguration());
        modelBuilder.ApplyConfiguration(new TagConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
