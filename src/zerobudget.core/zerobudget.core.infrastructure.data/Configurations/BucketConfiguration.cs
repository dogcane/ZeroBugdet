using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zerobudget.core.domain;

namespace zerobudget.core.infrastructure.data.Configurations;

public class BucketConfiguration : IEntityTypeConfiguration<Bucket>
{
    public void Configure(EntityTypeBuilder<Bucket> builder)
    {
        // Key configuration
        builder.HasKey(b => b.Identity);
        builder.Property(b => b.Identity)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        // Properties configuration
        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(b => b.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(b => b.DefaultLimit)
            .HasPrecision(18, 2);

        builder.Property(b => b.DefaultBalance)
            .HasPrecision(18, 2);

        builder.Property(b => b.Enabled)
            .HasDefaultValue(true);

        // Table configuration
        builder.ToTable("Buckets");
    }
}
