namespace zerobudget.core.identity.Commands;

/// <summary>
/// Command to register the main user (first user in the system)
/// </summary>
public record RegisterMainUserCommand(
    string Email,
    string Password,
    string ConfirmPassword
);

/// <summary>
/// Command to invite a user via email
/// </summary>
public record InviteUserCommand(
    string Email,
    string InvitedByUserId
);

/// <summary>
/// Command to complete user registration via invitation token
/// </summary>
public record CompleteUserRegistrationCommand(
    string Token,
    string Password,
    string ConfirmPassword
);

/// <summary>
/// Command to delete a user (physical deletion)
/// </summary>
public record DeleteUserCommand(
    string UserId,
    string RequestedByUserId
);
