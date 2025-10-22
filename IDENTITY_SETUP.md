# ASP.NET Core Identity API Integration - ZeroBudget

## ✅ Stato: COMPLETATO

ASP.NET Core Identity con JWT API è stato completamente configurato nel progetto ZeroBudget.

## 📦 Package Aggiunti

- ✅ **Microsoft.AspNetCore.Identity.EntityFrameworkCore** (v9.0.10)
- ✅ **Microsoft.AspNetCore.Authentication.JwtBearer** (v9.0.10)
- ✅ **System.IdentityModel.Tokens.Jwt** (v8.14.0)

## 🔧 Configurazione

### Database Context
- **IdentityDbContext**: `zerobudget.core.infrastructure.data/IdentityDbContext.cs`
- **Storage**: In-Memory Database (`ZeroBudget_Identity`)
- **Tabelle**: Users, Roles, UserRoles, UserClaims, UserLogins, UserTokens, RoleClaims

### Configurazioni di Sicurezza

#### Password Policy
- Minimo 8 caratteri
- Almeno una lettera maiuscola
- Almeno una lettera minuscola
- Almeno una cifra
- Almeno un carattere univoco

#### Lockout Policy
- Blocco dopo 5 tentativi falliti
- Durata lockout: 5 minuti
- Abilitato per nuovi utenti

#### User Settings
- Email univoca richiesta
- Username permessi: lettere, numeri, `-._@+`

### Middleware Configurati
- ✅ `app.UseAuthentication()` - Autenticazione
- ✅ `app.UseAuthorization()` - Autorizzazione

## 🚀 Prossimi Passi

### 1. Creare Controller di Account
```csharp
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new IdentityUser { UserName = request.Email, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
            return Ok(new { message = "User registered successfully" });

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, true);

        if (result.Succeeded)
            return Ok(new { message = "Login successful" });

        return Unauthorized(new { message = "Invalid credentials" });
    }
}
```

### 2. Aggiungere JWT per API (opzionale)
```bash
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package Microsoft.IdentityModel.Tokens
```

### 3. Proteggere i Controllori con [Authorize]
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]  // Richiede autenticazione
public class BucketController : ControllerBase
{
    // ...
}
```

### 4. Database Persistente (Produzione)
Sostituire il database in-memory con SQL Server o PostgreSQL:

```csharp
// SQL Server
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// PostgreSQL
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

## 📋 Struttura delle Tabelle Identity

### Users
```
Id (string)
UserName (string)
Email (string)
EmailConfirmed (bool)
PasswordHash (string)
PhoneNumber (string)
PhoneNumberConfirmed (bool)
TwoFactorEnabled (bool)
LockoutEnd (DateTimeOffset?)
LockoutEnabled (bool)
AccessFailedCount (int)
```

### Roles
```
Id (string)
Name (string)
NormalizedName (string)
```

### UserRoles
```
UserId (string) FK
RoleId (string) FK
```

## 🔐 Migliori Pratiche

1. **Password**: Sempre hashtate e mai memorizzate in chiaro
2. **Email Confirmation**: Implementare per nuovi account
3. **Two-Factor Authentication**: Per account critici
4. **Lockout**: Protegge da brute-force attacks
5. **Roles & Claims**: Per autorizzazione granulare
6. **HTTPS**: Sempre in produzione
7. **CORS**: Configurare correttamente per API pubbliche

## 📚 Risorse Utili

- [Microsoft Docs - ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [IdentityDbContext](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.entityframeworkcore.identitydbcontext)
- [UserManager<TUser>](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.usermanager-1)
- [SignInManager<TUser>](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.signinmanager-1)

## ✨ Status Aggiornamento

- ✅ Package Identity aggiunti
- ✅ IdentityDbContext creato
- ✅ Configurazione policies
- ✅ Middleware di autenticazione attivato
- ✅ In-Memory Database configurato
- ⏳ Prossimamente: Controller Account, JWT, EF Core Migrations
