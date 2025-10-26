using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Resulz;
using Wolverine;
using zerobudget.core.identity.Commands;
using zerobudget.core.identity.DTOs;
using zerobudget.core.identity.Entities;
using zerobudget.core.identity.Queries;
using zerobudget.core.identity.Services;

namespace zerobudget.core.identity.Controllers;

/// <summary>
/// Account controller for user authentication and registration
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AccountController> _logger;
    private readonly IMessageBus _messageBus;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        ILogger<AccountController> logger,
        IMessageBus messageBus)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _logger = logger;
        _messageBus = messageBus;
    }

    /// <summary>
    /// Check if main user registration is required
    /// </summary>
    [HttpGet("is-main-user-required")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> IsMainUserRequired()
    {
        var query = new IsMainUserRequiredQuery();
        var isRequired = await _messageBus.InvokeAsync<bool>(query);
        return Ok(new { isRequired });
    }

    /// <summary>
    /// Register the main user (first user in the system)
    /// </summary>
    [HttpPost("register-main-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterMainUser([FromBody] RegisterMainUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var command = new RegisterMainUserCommand(request.Email, request.Password, request.ConfirmPassword);
            var result = await _messageBus.InvokeAsync<OperationResult<UserDto>>(command);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    message = "Main user registration failed",
                    errors = result.Errors
                });
            }

            return Ok(new
            {
                message = "Main user registered successfully",
                user = result.Value
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Main user registration error: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Invite a user via email (only main user can invite)
    /// </summary>
    [HttpPost("invite-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> InviteUser([FromBody] InviteUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // Get the current user ID from claims
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { error = "User not authenticated" });

            var command = new InviteUserCommand(request.Email, userId);
            var result = await _messageBus.InvokeAsync<OperationResult<UserInvitationDto>>(command);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    message = "User invitation failed",
                    errors = result.Errors
                });
            }

            return Ok(new
            {
                message = "User invited successfully",
                invitation = result.Value
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"User invitation error: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Complete user registration via invitation token
    /// </summary>
    [HttpPost("complete-registration")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteRegistration([FromBody] CompleteRegistrationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var command = new CompleteUserRegistrationCommand(request.Token, request.Password, request.ConfirmPassword);
            var result = await _messageBus.InvokeAsync<OperationResult<UserDto>>(command);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    message = "Registration completion failed",
                    errors = result.Errors
                });
            }

            return Ok(new
            {
                message = "Registration completed successfully",
                user = result.Value
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Registration completion error: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Validate an invitation token
    /// </summary>
    [HttpGet("validate-invitation/{token}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ValidateInvitationToken(string token)
    {
        try
        {
            var query = new ValidateInvitationTokenQuery(token);
            var invitation = await _messageBus.InvokeAsync<UserInvitationDto?>(query);

            if (invitation == null)
                return NotFound(new { error = "Invalid invitation token" });

            if (invitation.IsUsed)
                return BadRequest(new { error = "This invitation has already been used" });

            if (invitation.ExpiresAt < DateTime.UtcNow)
                return BadRequest(new { error = "This invitation has expired" });

            return Ok(new
            {
                valid = true,
                email = invitation.Email,
                expiresAt = invitation.ExpiresAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Invitation validation error: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get all users (only main user can access)
    /// </summary>
    [HttpGet("users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { error = "User not authenticated" });

            // Verify user is main user
            var currentUser = await _userManager.FindByIdAsync(userId);
            if (currentUser == null || !currentUser.IsMainUser)
                return Unauthorized(new { error = "Only the main user can access user list" });

            var query = new GetAllUsersQuery();
            var users = await _messageBus.InvokeAsync<IEnumerable<UserDto>>(query);

            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get users error: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a user (only main user can delete, physical deletion)
    /// </summary>
    [HttpDelete("users/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        try
        {
            var requestingUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(requestingUserId))
                return Unauthorized(new { error = "User not authenticated" });

            var command = new DeleteUserCommand(userId, requestingUserId);
            var result = await _messageBus.InvokeAsync<OperationResult>(command);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    message = "User deletion failed",
                    errors = result.Errors
                });
            }

            return Ok(new { message = "User deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Delete user error: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get invitations sent by current user
    /// </summary>
    [HttpGet("invitations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetInvitations()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { error = "User not authenticated" });

            var query = new GetInvitationsByUserQuery(userId);
            var invitations = await _messageBus.InvokeAsync<IEnumerable<UserInvitationDto>>(query);

            return Ok(invitations);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get invitations error: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Register a new user (legacy endpoint, kept for backward compatibility)
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
            var user = new ApplicationUser
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
                roles = roles,
                isMainUser = user.IsMainUser
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
/// Request model for main user registration
/// </summary>
public class RegisterMainUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// Request model for user invitation
/// </summary>
public class InviteUserRequest
{
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Request model for completing registration via invitation
/// </summary>
public class CompleteRegistrationRequest
{
    public string Token { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
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
