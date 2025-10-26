using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using zerobudget.core.identity.Data;
using zerobudget.core.identity.DTOs;
using zerobudget.core.identity.Entities;
using zerobudget.core.identity.Queries;

namespace zerobudget.core.identity.Handlers.Queries;

/// <summary>
/// Handler for IsMainUserRequiredQuery
/// </summary>
public class IsMainUserRequiredQueryHandler(
    UserManager<ApplicationUser> userManager,
    ILogger<IsMainUserRequiredQueryHandler>? logger = null)
{
    public async Task<bool> Handle(IsMainUserRequiredQuery query)
    {
        var userCount = await userManager.Users.CountAsync();
        return userCount == 0;
    }
}

/// <summary>
/// Handler for GetAllUsersQuery
/// </summary>
public class GetAllUsersQueryHandler(
    UserManager<ApplicationUser> userManager,
    ILogger<GetAllUsersQueryHandler>? logger = null)
{
    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery query)
    {
        var users = await userManager.Users.ToListAsync();
        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Email = u.Email ?? string.Empty,
            UserName = u.UserName,
            IsMainUser = u.IsMainUser,
            CreatedAt = u.CreatedAt,
            InvitedByUserId = u.InvitedByUserId
        });
    }
}

/// <summary>
/// Handler for GetUserByIdQuery
/// </summary>
public class GetUserByIdQueryHandler(
    UserManager<ApplicationUser> userManager,
    ILogger<GetUserByIdQueryHandler>? logger = null)
{
    public async Task<UserDto?> Handle(GetUserByIdQuery query)
    {
        var user = await userManager.FindByIdAsync(query.UserId);
        if (user == null)
            return null;

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName,
            IsMainUser = user.IsMainUser,
            CreatedAt = user.CreatedAt,
            InvitedByUserId = user.InvitedByUserId
        };
    }
}

/// <summary>
/// Handler for ValidateInvitationTokenQuery
/// </summary>
public class ValidateInvitationTokenQueryHandler(
    ApplicationIdentityDbContext context,
    ILogger<ValidateInvitationTokenQueryHandler>? logger = null)
{
    public async Task<UserInvitationDto?> Handle(ValidateInvitationTokenQuery query)
    {
        var invitation = await context.UserInvitations
            .FirstOrDefaultAsync(i => i.Token == query.Token);

        if (invitation == null)
            return null;

        return new UserInvitationDto
        {
            Id = invitation.Id,
            Email = invitation.Email,
            Token = invitation.Token,
            CreatedAt = invitation.CreatedAt,
            ExpiresAt = invitation.ExpiresAt,
            IsUsed = invitation.IsUsed,
            UsedAt = invitation.UsedAt
        };
    }
}

/// <summary>
/// Handler for GetInvitationsByUserQuery
/// </summary>
public class GetInvitationsByUserQueryHandler(
    ApplicationIdentityDbContext context,
    ILogger<GetInvitationsByUserQueryHandler>? logger = null)
{
    public async Task<IEnumerable<UserInvitationDto>> Handle(GetInvitationsByUserQuery query)
    {
        var invitations = await context.UserInvitations
            .Where(i => i.InvitedByUserId == query.UserId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return invitations.Select(i => new UserInvitationDto
        {
            Id = i.Id,
            Email = i.Email,
            Token = i.Token,
            CreatedAt = i.CreatedAt,
            ExpiresAt = i.ExpiresAt,
            IsUsed = i.IsUsed,
            UsedAt = i.UsedAt
        });
    }
}
