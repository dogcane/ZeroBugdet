using Microsoft.AspNetCore.Identity;

namespace zerobudget.core.application.Entities;

/// <summary>
/// Custom user entity extending IdentityUser with additional properties for user management
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Indicates if this user is the main (primary) user who can manage other users
    /// </summary>
    public bool IsMainUser { get; set; }

    /// <summary>
    /// The ID of the main user who invited this user (null for main user)
    /// </summary>
    public string? InvitedByUserId { get; set; }

    /// <summary>
    /// Date and time when the user was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Navigation property to the user who invited this user
    /// </summary>
    public virtual ApplicationUser? InvitedBy { get; set; }

    /// <summary>
    /// Collection of users invited by this user
    /// </summary>
    public virtual ICollection<ApplicationUser> InvitedUsers { get; set; } = new List<ApplicationUser>();

    /// <summary>
    /// Collection of invitations sent by this user
    /// </summary>
    public virtual ICollection<UserInvitation> SentInvitations { get; set; } = new List<UserInvitation>();
}
