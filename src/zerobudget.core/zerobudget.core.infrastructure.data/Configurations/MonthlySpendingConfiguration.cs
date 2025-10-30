using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zerobudget.core.domain;

namespace zerobudget.core.infrastructure.data.Configurations;

public class MonthlySpendingConfiguration : IEntityTypeConfiguration<MonthlySpending>
{
    public void Configure(EntityTypeBuilder<MonthlySpending> builder)
    {
        // Key configuration
        builder.HasKey(ms => ms.Identity);
        builder.Property(ms => ms.Identity)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        // Properties configuration
        builder.Property(ms => ms.Date)
            .IsRequired();

        builder.Property(ms => ms.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(ms => ms.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(ms => ms.Owner)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ms => ms.Tags)
            .IsRequired()
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .HasMaxLength(4000);

        // Foreign key configuration
        builder.Property(ms => ms.MonthlyBucketId)
            .IsRequired();

        builder.HasOne<MonthlyBucket>()
            .WithMany()
            .HasForeignKey(ms => ms.MonthlyBucketId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(ms => ms.MonthlyBucketId);
        builder.HasIndex(ms => ms.Date);
        builder.HasIndex(ms => ms.Owner);

        // Table configuration
        builder.ToTable("MonthlySpendings");
    }
}
