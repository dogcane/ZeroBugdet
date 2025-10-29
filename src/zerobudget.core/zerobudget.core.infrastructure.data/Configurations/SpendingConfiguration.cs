using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zerobudget.core.domain;

namespace zerobudget.core.infrastructure.data.Configurations;

public class SpendingConfiguration : IEntityTypeConfiguration<Spending>
{
    public void Configure(EntityTypeBuilder<Spending> builder)
    {
        // Key configuration
        builder.HasKey(s => s.Identity);
        builder.Property(s => s.Identity)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        // Properties configuration
        builder.Property(s => s.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(s => s.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.Owner)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(s => s.Tags)
            .IsRequired()
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .HasMaxLength(4000);

        builder.Property(s => s.Enabled)
            .HasDefaultValue(true);

        // Foreign key configuration
        builder.Property(s => s.BucketId)
            .IsRequired();

        builder.HasOne<Bucket>()
            .WithMany()
            .HasForeignKey(s => s.BucketId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(s => s.BucketId);
        builder.HasIndex(s => s.Owner);
        builder.HasIndex(s => s.Enabled);

        // Table configuration
        builder.ToTable("Spendings");
    }
}
