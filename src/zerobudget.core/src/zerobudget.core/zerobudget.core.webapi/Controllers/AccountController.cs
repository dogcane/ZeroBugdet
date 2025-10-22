using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using zerobudget.core.webapi.Services;

namespace zerobudget.core.webapi.Controllers;

/// <summary>
/// Account controller for user authentication and registration
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ITokenService tokenService,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var user = new IdentityUser
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation($"User {request.Email} registered successfully");
                return Ok(new
                {
                    message = "User registered successfully",
                    userId = user.Id,
                    email = user.Email
                });
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning($"User registration failed: {errors}");
            return BadRequest(new { errors = result.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Registration error: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred during registration" });
        }
    }

    /// <summary>
    /// Login user and return JWT token
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning($"Login attempt with non-existent email: {request.Email}");
                return Unauthorized(new { error = "Invalid email or password" });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    _logger.LogWarning($"User {request.Email} is locked out");
                    return Unauthorized(new { error = "Account is locked due to too many failed login attempts" });
                }

                _logger.LogWarning($"Login failed for user: {request.Email}");
                return Unauthorized(new { error = "Invalid email or password" });
            }

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user.Id, user.Email ?? "", roles.ToArray());

            _logger.LogInformation($"User {request.Email} logged in successfully");
            return Ok(new
            {
                message = "Login successful",
                token = token,
                userId = user.Id,
                email = user.Email,
                roles = roles
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Login error: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred during login" });
        }
    }

    /// <summary>
    /// Logout user (API-based logout is typically just discarding the token on client side)
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out successfully");
            return Ok(new { message = "Logout successful" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Logout error: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred during logout" });
        }
    }
}

/// <summary>
/// Request model for user registration
/// </summary>
public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// Request model for user login
/// </summary>
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
