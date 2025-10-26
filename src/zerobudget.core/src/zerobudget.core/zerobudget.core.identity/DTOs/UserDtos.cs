namespace zerobudget.core.identity.DTOs;

/// <summary>
/// DTO for user information
/// </summary>
public record UserDto
{
    public string Id { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? UserName { get; init; }
    public bool IsMainUser { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? InvitedByUserId { get; init; }
}

/// <summary>
/// DTO for user invitation information
/// </summary>
public record UserInvitationDto
{
    public int Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; init; }
    public bool IsUsed { get; init; }
    public DateTime? UsedAt { get; init; }
}
