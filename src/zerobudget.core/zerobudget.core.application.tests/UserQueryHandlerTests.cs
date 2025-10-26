using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Xunit;
using zerobudget.core.application.Data;
using zerobudget.core.application.Entities;
using zerobudget.core.application.Handlers.Queries;
using zerobudget.core.application.Queries;

namespace zerobudget.core.application.tests;

public class UserQueryHandlerTests
{
    private ApplicationIdentityDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationIdentityDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationIdentityDbContext(options);
    }

    private Mock<UserManager<ApplicationUser>> CreateMockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var mockUserManager = new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        return mockUserManager;
    }

    [Fact]
    public async Task IsMainUserRequiredQuery_NoUsers_ShouldReturnTrue()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var handler = new IsMainUserRequiredQueryHandler(mockUserManager.Object);
        var query = new IsMainUserRequiredQuery();

        var users = new List<ApplicationUser>();
        var mockUsers = users.BuildMock();
        mockUserManager.Setup(m => m.Users)
            .Returns(mockUsers);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsMainUserRequiredQuery_UsersExist_ShouldReturnFalse()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var handler = new IsMainUserRequiredQueryHandler(mockUserManager.Object);
        var query = new IsMainUserRequiredQuery();

        var users = new List<ApplicationUser>
        {
            new ApplicationUser { Id = "1", Email = "user@example.com", IsMainUser = true }
        };

        var mockUsers = users.AsQueryable().BuildMock();
        mockUserManager.Setup(m => m.Users)
            .Returns(mockUsers);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAllUsersQuery_ShouldReturnAllUsers()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var handler = new GetAllUsersQueryHandler(mockUserManager.Object);
        var query = new GetAllUsersQuery();

        var users = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = "1",
                Email = "main@example.com",
                UserName = "main@example.com",
                IsMainUser = true,
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationUser
            {
                Id = "2",
                Email = "user@example.com",
                UserName = "user@example.com",
                IsMainUser = false,
                CreatedAt = DateTime.UtcNow,
                InvitedByUserId = "1"
            }
        };

        var mockUsers = users.BuildMock();
        mockUserManager.Setup(m => m.Users)
            .Returns(mockUsers);

        // Act
        var result = await handler.Handle(query);

        // Assert
        var userList = result.ToList();
        Assert.Equal(2, userList.Count);
        Assert.Contains(userList, u => u.Email == "main@example.com" && u.IsMainUser);
        Assert.Contains(userList, u => u.Email == "user@example.com" && !u.IsMainUser);
    }

    [Fact]
    public async Task GetUserByIdQuery_UserExists_ShouldReturnUser()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var handler = new GetUserByIdQueryHandler(mockUserManager.Object);
        var query = new GetUserByIdQuery("user-id");

        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = "user@example.com",
            UserName = "user@example.com",
            IsMainUser = false,
            CreatedAt = DateTime.UtcNow
        };

        mockUserManager.Setup(m => m.FindByIdAsync("user-id"))
            .ReturnsAsync(user);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("user-id", result!.Id);
        Assert.Equal("user@example.com", result.Email);
    }

    [Fact]
    public async Task GetUserByIdQuery_UserNotFound_ShouldReturnNull()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var handler = new GetUserByIdQueryHandler(mockUserManager.Object);
        var query = new GetUserByIdQuery("non-existent-id");

        mockUserManager.Setup(m => m.FindByIdAsync("non-existent-id"))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ValidateInvitationTokenQuery_ValidToken_ShouldReturnInvitation()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var handler = new ValidateInvitationTokenQueryHandler(context);
        var query = new ValidateInvitationTokenQuery("valid-token");

        var invitation = new UserInvitation
        {
            Email = "invited@example.com",
            Token = "valid-token",
            InvitedByUserId = "main-user-id",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(48),
            IsUsed = false
        };

        context.UserInvitations.Add(invitation);
        await context.SaveChangesAsync();

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("valid-token", result!.Token);
        Assert.Equal("invited@example.com", result.Email);
        Assert.False(result.IsUsed);
    }

    [Fact]
    public async Task ValidateInvitationTokenQuery_InvalidToken_ShouldReturnNull()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var handler = new ValidateInvitationTokenQueryHandler(context);
        var query = new ValidateInvitationTokenQuery("invalid-token");

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetInvitationsByUserQuery_ShouldReturnUserInvitations()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var handler = new GetInvitationsByUserQueryHandler(context);
        var query = new GetInvitationsByUserQuery("main-user-id");

        var invitations = new List<UserInvitation>
        {
            new UserInvitation
            {
                Email = "user1@example.com",
                Token = "token1",
                InvitedByUserId = "main-user-id",
                CreatedAt = DateTime.UtcNow.AddHours(-10),
                ExpiresAt = DateTime.UtcNow.AddHours(38),
                IsUsed = false
            },
            new UserInvitation
            {
                Email = "user2@example.com",
                Token = "token2",
                InvitedByUserId = "main-user-id",
                CreatedAt = DateTime.UtcNow.AddHours(-5),
                ExpiresAt = DateTime.UtcNow.AddHours(43),
                IsUsed = true,
                UsedAt = DateTime.UtcNow.AddHours(-3)
            },
            new UserInvitation
            {
                Email = "other@example.com",
                Token = "token3",
                InvitedByUserId = "other-user-id",
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(48),
                IsUsed = false
            }
        };

        context.UserInvitations.AddRange(invitations);
        await context.SaveChangesAsync();

        // Act
        var result = await handler.Handle(query);

        // Assert
        var invitationList = result.ToList();
        Assert.Equal(2, invitationList.Count); // Should only return invitations by main-user-id
        Assert.All(invitationList, inv => Assert.NotEqual("other@example.com", inv.Email));
        Assert.Contains(invitationList, inv => inv.Email == "user1@example.com");
        Assert.Contains(invitationList, inv => inv.Email == "user2@example.com" && inv.IsUsed);
    }

    [Fact]
    public async Task GetInvitationsByUserQuery_NoInvitations_ShouldReturnEmpty()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var handler = new GetInvitationsByUserQueryHandler(context);
        var query = new GetInvitationsByUserQuery("user-with-no-invitations");

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.Empty(result);
    }
}
