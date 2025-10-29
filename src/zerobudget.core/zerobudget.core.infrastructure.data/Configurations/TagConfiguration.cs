using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zerobudget.core.domain;

namespace zerobudget.core.infrastructure.data.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        // Key configuration
        builder.HasKey(t => t.Identity);
        builder.Property(t => t.Identity)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        // Properties configuration
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(255);

        // Indexes
        builder.HasIndex(t => t.Name)
            .IsUnique();

        // Table configuration
        builder.ToTable("Tags");
    }
}
