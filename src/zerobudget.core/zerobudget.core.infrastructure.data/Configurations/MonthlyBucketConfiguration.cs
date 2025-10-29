using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zerobudget.core.domain;

namespace zerobudget.core.infrastructure.data.Configurations;

public class MonthlyBucketConfiguration : IEntityTypeConfiguration<MonthlyBucket>
{
    public void Configure(EntityTypeBuilder<MonthlyBucket> builder)
    {
        // Key configuration
        builder.HasKey(mb => mb.Identity);
        builder.Property(mb => mb.Identity)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        // Properties configuration
        builder.Property(mb => mb.Year)
            .IsRequired();

        builder.Property(mb => mb.Month)
            .IsRequired();

        builder.Property(mb => mb.Balance)
            .HasPrecision(18, 2);

        builder.Property(mb => mb.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(mb => mb.Limit)
            .HasPrecision(18, 2);

        // Foreign key configuration
        builder.Property("BucketId")
            .IsRequired();

        builder.HasOne(mb => mb.Bucket)
            .WithMany()
            .HasForeignKey("BucketId")
            .OnDelete(DeleteBehavior.Cascade);

        // Relationships
        builder.HasMany<MonthlySpending>()
            .WithOne()
            .HasForeignKey("MonthlyBucketId")
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex("BucketId", "Year", "Month")
            .IsUnique();

        // Table configuration
        builder.ToTable("MonthlyBuckets");
    }
}
