using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Resulz;
using zerobudget.core.application.Commands;
using zerobudget.core.application.Data;
using zerobudget.core.application.DTOs;
using zerobudget.core.application.Entities;

namespace zerobudget.core.application.Handlers.Commands;

/// <summary>
/// Handler for RegisterMainUserCommand
/// </summary>
public class RegisterMainUserCommandHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RegisterMainUserCommandHandler>? _logger;

    public RegisterMainUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<RegisterMainUserCommandHandler>? logger = null)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<OperationResult<UserDto>> Handle(RegisterMainUserCommand command)
    {
        // Check if any users exist
        var userCount = await _userManager.Users.CountAsync();
        if (userCount > 0)
        {
            return OperationResult<UserDto>.MakeFailure(
                ErrorMessage.Create("MAIN_USER_EXISTS", "Main user already exists"));
        }

        // Validate passwords match
        if (command.Password != command.ConfirmPassword)
        {
            return OperationResult<UserDto>.MakeFailure(
                ErrorMessage.Create("PASSWORD_MISMATCH", "Password and confirmation password do not match"));
        }

        // Create the main user
        var user = new ApplicationUser
        {
            UserName = command.Email,
            Email = command.Email,
            IsMainUser = true,
            CreatedAt = DateTime.UtcNow,
            EmailConfirmed = true // Auto-confirm main user
        };

        var result = await _userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => 
                ErrorMessage.Create(e.Code, e.Description)).ToArray();
            return OperationResult<UserDto>.MakeFailure(errors);
        }

        _logger?.LogInformation($"Main user {command.Email} registered successfully");

        return OperationResult<UserDto>.MakeSuccess(new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName,
            IsMainUser = user.IsMainUser,
            CreatedAt = user.CreatedAt,
            InvitedByUserId = user.InvitedByUserId
        });
    }
}

/// <summary>
/// Handler for InviteUserCommand
/// </summary>
public class InviteUserCommandHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationIdentityDbContext _context;
    private readonly ILogger<InviteUserCommandHandler>? _logger;

    public InviteUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        ApplicationIdentityDbContext context,
        ILogger<InviteUserCommandHandler>? logger = null)
    {
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }

    public async Task<OperationResult<UserInvitationDto>> Handle(InviteUserCommand command)
    {
        // Verify the inviting user is the main user
        var invitingUser = await _userManager.FindByIdAsync(command.InvitedByUserId);
        if (invitingUser == null || !invitingUser.IsMainUser)
        {
            return OperationResult<UserInvitationDto>.MakeFailure(
                ErrorMessage.Create("UNAUTHORIZED", "Only the main user can invite other users"));
        }

        // Check if email already exists as a user
        var existingUser = await _userManager.FindByEmailAsync(command.Email);
        if (existingUser != null)
        {
            return OperationResult<UserInvitationDto>.MakeFailure(
                ErrorMessage.Create("USER_EXISTS", "A user with this email already exists"));
        }

        // Check if there's already a pending invitation for this email
        var existingInvitation = await _context.UserInvitations
            .FirstOrDefaultAsync(i => i.Email == command.Email && !i.IsUsed && i.ExpiresAt > DateTime.UtcNow);
        if (existingInvitation != null)
        {
            return OperationResult<UserInvitationDto>.MakeFailure(
                ErrorMessage.Create("INVITATION_EXISTS", "An active invitation for this email already exists"));
        }

        // Count total users (excluding main user) and pending invitations
        var totalUsers = await _userManager.Users.CountAsync(u => !u.IsMainUser);
        var pendingInvitations = await _context.UserInvitations
            .CountAsync(i => !i.IsUsed && i.ExpiresAt > DateTime.UtcNow);
        
        if (totalUsers + pendingInvitations >= 5)
        {
            return OperationResult<UserInvitationDto>.MakeFailure(
                ErrorMessage.Create("USER_LIMIT_REACHED", "Maximum number of users (5) reached"));
        }

        // Create the invitation
        var invitation = new UserInvitation
        {
            Email = command.Email,
            Token = Guid.NewGuid().ToString("N"),
            InvitedByUserId = command.InvitedByUserId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(48),
            IsUsed = false
        };

        _context.UserInvitations.Add(invitation);
        await _context.SaveChangesAsync();

        _logger?.LogInformation($"User {command.Email} invited by {command.InvitedByUserId}");

        return OperationResult<UserInvitationDto>.MakeSuccess(new UserInvitationDto
        {
            Id = invitation.Id,
            Email = invitation.Email,
            Token = invitation.Token,
            CreatedAt = invitation.CreatedAt,
            ExpiresAt = invitation.ExpiresAt,
            IsUsed = invitation.IsUsed,
            UsedAt = invitation.UsedAt
        });
    }
}

/// <summary>
/// Handler for CompleteUserRegistrationCommand
/// </summary>
public class CompleteUserRegistrationCommandHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationIdentityDbContext _context;
    private readonly ILogger<CompleteUserRegistrationCommandHandler>? _logger;

    public CompleteUserRegistrationCommandHandler(
        UserManager<ApplicationUser> userManager,
        ApplicationIdentityDbContext context,
        ILogger<CompleteUserRegistrationCommandHandler>? logger = null)
    {
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }

    public async Task<OperationResult<UserDto>> Handle(CompleteUserRegistrationCommand command)
    {
        // Validate passwords match
        if (command.Password != command.ConfirmPassword)
        {
            return OperationResult<UserDto>.MakeFailure(
                ErrorMessage.Create("PASSWORD_MISMATCH", "Password and confirmation password do not match"));
        }

        // Find and validate the invitation
        var invitation = await _context.UserInvitations
            .FirstOrDefaultAsync(i => i.Token == command.Token);

        if (invitation == null)
        {
            return OperationResult<UserDto>.MakeFailure(
                ErrorMessage.Create("INVALID_TOKEN", "Invalid invitation token"));
        }

        if (invitation.IsUsed)
        {
            return OperationResult<UserDto>.MakeFailure(
                ErrorMessage.Create("TOKEN_USED", "This invitation has already been used"));
        }

        if (invitation.ExpiresAt < DateTime.UtcNow)
        {
            return OperationResult<UserDto>.MakeFailure(
                ErrorMessage.Create("TOKEN_EXPIRED", "This invitation has expired"));
        }

        // Check if email already exists as a user (race condition check)
        var existingUser = await _userManager.FindByEmailAsync(invitation.Email);
        if (existingUser != null)
        {
            return OperationResult<UserDto>.MakeFailure(
                ErrorMessage.Create("USER_EXISTS", "A user with this email already exists"));
        }

        // Create the user
        var user = new ApplicationUser
        {
            UserName = invitation.Email,
            Email = invitation.Email,
            IsMainUser = false,
            InvitedByUserId = invitation.InvitedByUserId,
            CreatedAt = DateTime.UtcNow,
            EmailConfirmed = true // Auto-confirm invited users
        };

        var result = await _userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => 
                ErrorMessage.Create(e.Code, e.Description)).ToArray();
            return OperationResult<UserDto>.MakeFailure(errors);
        }

        // Mark invitation as used
        invitation.IsUsed = true;
        invitation.UsedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger?.LogInformation($"User {invitation.Email} completed registration via invitation");

        return OperationResult<UserDto>.MakeSuccess(new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName,
            IsMainUser = user.IsMainUser,
            CreatedAt = user.CreatedAt,
            InvitedByUserId = user.InvitedByUserId
        });
    }
}

/// <summary>
/// Handler for DeleteUserCommand
/// </summary>
public class DeleteUserCommandHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DeleteUserCommandHandler>? _logger;

    public DeleteUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<DeleteUserCommandHandler>? logger = null)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<OperationResult> Handle(DeleteUserCommand command)
    {
        // Verify the requesting user is the main user
        var requestingUser = await _userManager.FindByIdAsync(command.RequestedByUserId);
        if (requestingUser == null || !requestingUser.IsMainUser)
        {
            return OperationResult.MakeFailure(
                ErrorMessage.Create("UNAUTHORIZED", "Only the main user can delete other users"));
        }

        // Get the user to delete
        var userToDelete = await _userManager.FindByIdAsync(command.UserId);
        if (userToDelete == null)
        {
            return OperationResult.MakeFailure(
                ErrorMessage.Create("USER_NOT_FOUND", "User not found"));
        }

        // Prevent deleting the main user
        if (userToDelete.IsMainUser)
        {
            return OperationResult.MakeFailure(
                ErrorMessage.Create("CANNOT_DELETE_MAIN_USER", "Cannot delete the main user"));
        }

        // Delete the user
        var result = await _userManager.DeleteAsync(userToDelete);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => 
                ErrorMessage.Create(e.Code, e.Description)).ToArray();
            return OperationResult.MakeFailure(errors);
        }

        _logger?.LogInformation($"User {userToDelete.Email} deleted by {requestingUser.Email}");

        return OperationResult.MakeSuccess();
    }
}
