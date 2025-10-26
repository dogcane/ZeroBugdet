namespace zerobudget.core.identity.Queries;

/// <summary>
/// Query to check if the main user registration is required (no users exist)
/// </summary>
public record IsMainUserRequiredQuery();

/// <summary>
/// Query to get all users in the system
/// </summary>
public record GetAllUsersQuery();

/// <summary>
/// Query to get a specific user by ID
/// </summary>
public record GetUserByIdQuery(string UserId);

/// <summary>
/// Query to validate an invitation token
/// </summary>
public record ValidateInvitationTokenQuery(string Token);

/// <summary>
/// Query to get all invitations sent by a user
/// </summary>
public record GetInvitationsByUserQuery(string UserId);
