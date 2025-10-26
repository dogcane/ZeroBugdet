using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using zerobudget.core.application.Entities;

namespace zerobudget.core.application.Data;

/// <summary>
/// DbContext for managing Identity (users, roles, claims, etc.) with custom ApplicationUser
/// </summary>
public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// User invitations
    /// </summary>
    public DbSet<UserInvitation> UserInvitations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure table names
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable(name: "Users");
        });

        builder.Entity<IdentityRole>(entity =>
        {
            entity.ToTable(name: "Roles");
        });

        builder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("UserRoles");
        });

        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("UserClaims");
        });

        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("UserLogins");
        });

        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("UserTokens");
        });

        builder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("RoleClaims");
        });

        // Configure UserInvitation
        builder.Entity<UserInvitation>(entity =>
        {
            entity.ToTable("UserInvitations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(256);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => e.Email);
        });
    }
}
