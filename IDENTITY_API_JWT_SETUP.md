# ASP.NET Core Identity API with JWT - ZeroBudget

## ✅ Status: FULLY IMPLEMENTED

ASP.NET Core Identity with JWT API has been completely configured for the ZeroBudget project.

## 📦 Packages Installed

- ✅ **Microsoft.AspNetCore.Identity.EntityFrameworkCore** (v9.0.10)
- ✅ **Microsoft.AspNetCore.Authentication.JwtBearer** (v9.0.10)
- ✅ **System.IdentityModel.Tokens.Jwt** (v8.14.0)

## 🔑 JWT Configuration

### appsettings.json
```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-change-in-production-at-least-32-chars-long-key",
    "Issuer": "ZeroBudgetAPI",
    "Audience": "ZeroBudgetClients",
    "ExpirationMinutes": 60
  }
}
```

**Important**: Change the SecretKey to a strong, randomly generated string in production!

## 🏗️ Architecture

### Database Context
- **IdentityDbContext**: `zerobudget.core.infrastructure.data/IdentityDbContext.cs`
- **Storage**: In-Memory Database (development)
- **Tables**: Users, Roles, UserRoles, UserClaims, UserLogins, UserTokens, RoleClaims

### Services
- **ITokenService / JwtTokenService**: Generates and validates JWT tokens
- **Location**: `Services/JwtTokenService.cs`

### Controllers
- **AccountController**: User registration, login, logout
- **Location**: `Controllers/AccountController.cs`

## 🔐 Security Configuration

### Password Policy
- Minimum 8 characters
- Requires uppercase letter
- Requires lowercase letter
- Requires digit
- At least 1 unique character

### Lockout Policy
- 5 failed login attempts trigger lockout
- Lockout duration: 5 minutes
- Enabled for new users

### JWT Settings
- Token expiration: 60 minutes
- Algorithm: HS256 (HMAC SHA256)
- Validates issuer, audience, and lifetime

## 🚀 API Endpoints

### POST /api/account/register
Register a new user

**Request:**
```json
{
  "email": "user@example.com",
  "password": "SecurePass123",
  "confirmPassword": "SecurePass123"
}
```

**Response (200):**
```json
{
  "message": "User registered successfully",
  "userId": "abc123",
  "email": "user@example.com"
}
```

**Responses:**
- `200 OK` - Registration successful
- `400 Bad Request` - Invalid input or user already exists

### POST /api/account/login
Authenticate user and receive JWT token

**Request:**
```json
{
  "email": "user@example.com",
  "password": "SecurePass123"
}
```

**Response (200):**
```json
{
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": "abc123",
  "email": "user@example.com",
  "roles": ["user", "admin"]
}
```

**Responses:**
- `200 OK` - Login successful
- `401 Unauthorized` - Invalid credentials or account locked
- `400 Bad Request` - Invalid input

### POST /api/account/logout
Logout user (invalidates session)

**Response (200):**
```json
{
  "message": "Logout successful"
}
```

## 💡 Usage Examples

### Using the HTTP Client (VS Code)

Add to `zerobudget.core.webapi.http`:

```http
# Account - Register
POST {{baseUrl}}/api/account/register
Content-Type: application/json

{
  "email": "testuser@example.com",
  "password": "TestPass123",
  "confirmPassword": "TestPass123"
}

### Account - Login
POST {{baseUrl}}/api/account/login
Content-Type: application/json

{
  "email": "testuser@example.com",
  "password": "TestPass123"
}

### Account - Logout
POST {{baseUrl}}/api/account/logout
```

### Using cURL

**Register:**
```bash
curl -X POST http://localhost:5143/api/account/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "MyPassword123",
    "confirmPassword": "MyPassword123"
  }'
```

**Login:**
```bash
curl -X POST http://localhost:5143/api/account/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "MyPassword123"
  }'
```

**Call Protected Endpoint:**
```bash
curl -X GET http://localhost:5143/api/bucket \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
```

## 🔒 Protecting Endpoints

### Require Authentication
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]  // All endpoints require JWT token
public class BucketController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<BucketDto>> GetById(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // ... protected logic
    }
}
```

### Allow Anonymous Access
```csharp
[AllowAnonymous]
[HttpGet("public")]
public IActionResult GetPublic()
{
    return Ok("This endpoint is public");
}
```

### Role-Based Protection
```csharp
[Authorize(Roles = "Admin")]
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    // Only Admin role can access
}
```

## 👥 Managing Roles

### Add User to Role
```csharp
var user = await _userManager.FindByEmailAsync("user@example.com");
await _userManager.AddToRoleAsync(user, "Admin");
```

### Create Role (in Startup)
```csharp
var roleManager = app.Services.GetRequiredService<RoleManager<IdentityRole>>();
if (!await roleManager.RoleExistsAsync("Admin"))
{
    await roleManager.CreateAsync(new IdentityRole("Admin"));
}
```

### Check User Roles
```csharp
var roles = await _userManager.GetRolesAsync(user);
```

## 🔄 Token Information

### JWT Token Structure
```
Header.Payload.Signature
```

**Payload (decoded) contains:**
```json
{
  "nameid": "user-id-here",
  "email": "user@example.com",
  "role": ["user", "admin"],
  "exp": 1697890123,
  "iss": "ZeroBudgetAPI",
  "aud": "ZeroBudgetClients"
}
```

### Token Validation
- **Signature**: Verified using secret key
- **Issuer**: Must match configured issuer
- **Audience**: Must match configured audience
- **Expiration**: Must not be expired
- **Clock Skew**: 0 seconds (strict validation)

## 🔧 Advanced Configuration

### Change Token Expiration
Edit `appsettings.json`:
```json
"JwtSettings": {
  "ExpirationMinutes": 120  // 2 hours
}
```

### Use Persistent Database (Production)

**SQL Server:**
```csharp
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

**PostgreSQL:**
```csharp
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### Implement Refresh Tokens
```csharp
[HttpPost("refresh")]
public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
{
    // 1. Validate refresh token
    // 2. Generate new access token
    // 3. Return new token
}
```

## 📊 Database Schema

### Users Table
```
Id (string)
UserName (string)
Email (string)
EmailConfirmed (bool)
PasswordHash (string)
PhoneNumber (string)
TwoFactorEnabled (bool)
LockoutEnd (DateTimeOffset?)
LockoutEnabled (bool)
AccessFailedCount (int)
```

### Roles Table
```
Id (string)
Name (string)
NormalizedName (string)
```

### UserRoles Table
```
UserId (string) FK -> Users
RoleId (string) FK -> Roles
```

## 🛡️ Security Best Practices

1. **Secret Key**: Generate strong, random 32+ character keys
   ```bash
   # Generate secure key (on Linux/Mac)
   openssl rand -base64 32
   ```

2. **HTTPS**: Always use HTTPS in production
3. **Token Storage**: Store JWT in secure HTTP-only cookies (not localStorage)
4. **Expiration**: Keep token expiration short (30-60 minutes)
5. **Refresh Tokens**: Implement refresh token rotation for long-lived sessions
6. **Rate Limiting**: Implement rate limiting on login/register endpoints
7. **Logging**: Log authentication failures for security monitoring
8. **CORS**: Configure CORS properly for API security

## 📚 Files Overview

| File | Purpose |
|------|---------|
| `Program.cs` | JWT and Identity configuration |
| `appsettings.json` | JWT settings |
| `Services/JwtTokenService.cs` | Token generation and validation |
| `Controllers/AccountController.cs` | User registration/login/logout |
| `IdentityDbContext.cs` | Identity database context |

## ✨ Implementation Checklist

- ✅ JWT Bearer authentication configured
- ✅ Identity DbContext created
- ✅ Token service implemented
- ✅ Account controller created
- ✅ Middleware configured
- ✅ Security policies applied
- ✅ HTTP client examples ready
- ⏳ Refresh tokens (optional)
- ⏳ Two-factor authentication (optional)
- ⏳ Email confirmation (optional)

## 📞 Support

For more information, refer to:
- [Microsoft Docs - ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [JWT.io](https://jwt.io)
- [Microsoft Identity Model Tokens](https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet)
