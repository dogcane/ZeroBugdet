namespace zerobudget.core.application.Entities;

/// <summary>
/// Represents an invitation sent to a user to join the application
/// </summary>
public class UserInvitation
{
    /// <summary>
    /// Unique identifier for the invitation
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Email address of the invited user
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Unique token for the invitation (used in the registration link)
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// ID of the user who sent the invitation
    /// </summary>
    public string InvitedByUserId { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the invitation was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the invitation expires (48 hours from creation)
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Indicates if the invitation has been used
    /// </summary>
    public bool IsUsed { get; set; }

    /// <summary>
    /// Date and time when the invitation was used (null if not used)
    /// </summary>
    public DateTime? UsedAt { get; set; }

    /// <summary>
    /// Navigation property to the user who sent the invitation
    /// </summary>
    public virtual ApplicationUser InvitedBy { get; set; } = null!;
}
