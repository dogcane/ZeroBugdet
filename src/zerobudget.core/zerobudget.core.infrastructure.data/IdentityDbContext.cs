using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace zerobudget.core.infrastructure.data;

/// <summary>
/// DbContext per la gestione dell'Identity (utenti, ruoli, claims, ecc.)
/// </summary>
public class IdentityDbContext(DbContextOptions<IdentityDbContext> options)
    : IdentityDbContext<IdentityUser, IdentityRole, string>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configurazioni personalizzate per le entità di Identity, se necessarie
        // Ad esempio, renomina delle tabelle:
        builder.Entity<IdentityUser>(entity =>
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
    }
}
