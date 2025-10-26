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
public class IsMainUserRequiredQueryHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<IsMainUserRequiredQueryHandler>? _logger;

    public IsMainUserRequiredQueryHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<IsMainUserRequiredQueryHandler>? logger = null)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<bool> Handle(IsMainUserRequiredQuery query)
    {
        var userCount = await _userManager.Users.CountAsync();
        return userCount == 0;
    }
}

/// <summary>
/// Handler for GetAllUsersQuery
/// </summary>
public class GetAllUsersQueryHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GetAllUsersQueryHandler>? _logger;

    public GetAllUsersQueryHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<GetAllUsersQueryHandler>? logger = null)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery query)
    {
        var users = await _userManager.Users.ToListAsync();
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
public class GetUserByIdQueryHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GetUserByIdQueryHandler>? _logger;

    public GetUserByIdQueryHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<GetUserByIdQueryHandler>? logger = null)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery query)
    {
        var user = await _userManager.FindByIdAsync(query.UserId);
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
public class ValidateInvitationTokenQueryHandler
{
    private readonly ApplicationIdentityDbContext _context;
    private readonly ILogger<ValidateInvitationTokenQueryHandler>? _logger;

    public ValidateInvitationTokenQueryHandler(
        ApplicationIdentityDbContext context,
        ILogger<ValidateInvitationTokenQueryHandler>? logger = null)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UserInvitationDto?> Handle(ValidateInvitationTokenQuery query)
    {
        var invitation = await _context.UserInvitations
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
public class GetInvitationsByUserQueryHandler
{
    private readonly ApplicationIdentityDbContext _context;
    private readonly ILogger<GetInvitationsByUserQueryHandler>? _logger;

    public GetInvitationsByUserQueryHandler(
        ApplicationIdentityDbContext context,
        ILogger<GetInvitationsByUserQueryHandler>? logger = null)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<UserInvitationDto>> Handle(GetInvitationsByUserQuery query)
    {
        var invitations = await _context.UserInvitations
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
