using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Xunit;
using zerobudget.core.application.Commands;
using zerobudget.core.application.Data;
using zerobudget.core.application.Entities;
using zerobudget.core.application.Handlers.Commands;

namespace zerobudget.core.application.tests;

public class UserCommandHandlerTests
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
    public async Task RegisterMainUserCommand_NoUsersExist_ShouldSucceed()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var handler = new RegisterMainUserCommandHandler(mockUserManager.Object);
        var command = new RegisterMainUserCommand("test@example.com", "Password123!", "Password123!");

        var users = new List<ApplicationUser>();
        var mockUsers = users.AsQueryable();

        mockUserManager.Setup(m => m.Users)
            .Returns(mockUsers);

        mockUserManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("test@example.com", result.Value!.Email);
        Assert.True(result.Value.IsMainUser);
        mockUserManager.Verify(m => m.CreateAsync(It.Is<ApplicationUser>(u => u.IsMainUser && u.Email == "test@example.com"), "Password123!"), Times.Once);
    }

    [Fact]
    public async Task RegisterMainUserCommand_UsersAlreadyExist_ShouldFail()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var handler = new RegisterMainUserCommandHandler(mockUserManager.Object);
        var command = new RegisterMainUserCommand("test@example.com", "Password123!", "Password123!");

        var existingUsers = new List<ApplicationUser>
        {
            new ApplicationUser { Email = "existing@example.com", IsMainUser = true }
        };

        var mockUsers = existingUsers.AsQueryable();
        mockUserManager.Setup(m => m.Users)
            .Returns(mockUsers);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task RegisterMainUserCommand_PasswordMismatch_ShouldFail()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var handler = new RegisterMainUserCommandHandler(mockUserManager.Object);
        var command = new RegisterMainUserCommand("test@example.com", "Password123!", "DifferentPassword");

        var users = new List<ApplicationUser>();
        var mockUsers = users.AsQueryable();

        mockUserManager.Setup(m => m.Users)
            .Returns(mockUsers);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task InviteUserCommand_MainUserInviting_ShouldSucceed()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var context = CreateInMemoryContext();
        var handler = new InviteUserCommandHandler(mockUserManager.Object, context);

        var mainUser = new ApplicationUser
        {
            Id = "main-user-id",
            Email = "main@example.com",
            IsMainUser = true
        };

        mockUserManager.Setup(m => m.FindByIdAsync("main-user-id"))
            .ReturnsAsync(mainUser);

        mockUserManager.Setup(m => m.FindByEmailAsync("invited@example.com"))
            .ReturnsAsync((ApplicationUser?)null);

        var users = new List<ApplicationUser> { mainUser };
        var mockUsers = users.AsQueryable();
        mockUserManager.Setup(m => m.Users)
            .Returns(mockUsers);

        var command = new InviteUserCommand("invited@example.com", "main-user-id");

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("invited@example.com", result.Value!.Email);
        Assert.NotNull(result.Value.Token);
        Assert.True(result.Value.ExpiresAt > DateTime.UtcNow);
        Assert.True(result.Value.ExpiresAt <= DateTime.UtcNow.AddHours(48.1));
    }

    [Fact]
    public async Task InviteUserCommand_NonMainUserInviting_ShouldFail()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var context = CreateInMemoryContext();
        var handler = new InviteUserCommandHandler(mockUserManager.Object, context);

        var regularUser = new ApplicationUser
        {
            Id = "regular-user-id",
            Email = "regular@example.com",
            IsMainUser = false
        };

        mockUserManager.Setup(m => m.FindByIdAsync("regular-user-id"))
            .ReturnsAsync(regularUser);

        var command = new InviteUserCommand("invited@example.com", "regular-user-id");

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task InviteUserCommand_UserLimitReached_ShouldFail()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var context = CreateInMemoryContext();
        var handler = new InviteUserCommandHandler(mockUserManager.Object, context);

        var mainUser = new ApplicationUser
        {
            Id = "main-user-id",
            Email = "main@example.com",
            IsMainUser = true
        };

        // Create 5 non-main users
        var users = new List<ApplicationUser> { mainUser };
        for (int i = 0; i < 5; i++)
        {
            users.Add(new ApplicationUser
            {
                Id = $"user-{i}",
                Email = $"user{i}@example.com",
                IsMainUser = false
            });
        }

        mockUserManager.Setup(m => m.FindByIdAsync("main-user-id"))
            .ReturnsAsync(mainUser);

        mockUserManager.Setup(m => m.FindByEmailAsync("newuser@example.com"))
            .ReturnsAsync((ApplicationUser?)null);

        var mockUsers = users.AsQueryable();
        mockUserManager.Setup(m => m.Users)
            .Returns(mockUsers);

        var command = new InviteUserCommand("newuser@example.com", "main-user-id");

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task CompleteUserRegistrationCommand_ValidToken_ShouldSucceed()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var context = CreateInMemoryContext();
        var handler = new CompleteUserRegistrationCommandHandler(mockUserManager.Object, context);

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

        mockUserManager.Setup(m => m.FindByEmailAsync("invited@example.com"))
            .ReturnsAsync((ApplicationUser?)null);

        mockUserManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var command = new CompleteUserRegistrationCommand("valid-token", "Password123!", "Password123!");

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("invited@example.com", result.Value!.Email);
        Assert.False(result.Value.IsMainUser);
        Assert.Equal("main-user-id", result.Value.InvitedByUserId);

        // Verify invitation is marked as used
        var updatedInvitation = await context.UserInvitations.FindAsync(invitation.Id);
        Assert.True(updatedInvitation!.IsUsed);
        Assert.NotNull(updatedInvitation.UsedAt);
    }

    [Fact]
    public async Task CompleteUserRegistrationCommand_ExpiredToken_ShouldFail()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var context = CreateInMemoryContext();
        var handler = new CompleteUserRegistrationCommandHandler(mockUserManager.Object, context);

        var invitation = new UserInvitation
        {
            Email = "invited@example.com",
            Token = "expired-token",
            InvitedByUserId = "main-user-id",
            CreatedAt = DateTime.UtcNow.AddHours(-50),
            ExpiresAt = DateTime.UtcNow.AddHours(-2), // Expired
            IsUsed = false
        };

        context.UserInvitations.Add(invitation);
        await context.SaveChangesAsync();

        var command = new CompleteUserRegistrationCommand("expired-token", "Password123!", "Password123!");

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task CompleteUserRegistrationCommand_UsedToken_ShouldFail()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var context = CreateInMemoryContext();
        var handler = new CompleteUserRegistrationCommandHandler(mockUserManager.Object, context);

        var invitation = new UserInvitation
        {
            Email = "invited@example.com",
            Token = "used-token",
            InvitedByUserId = "main-user-id",
            CreatedAt = DateTime.UtcNow.AddHours(-10),
            ExpiresAt = DateTime.UtcNow.AddHours(38),
            IsUsed = true, // Already used
            UsedAt = DateTime.UtcNow.AddHours(-5)
        };

        context.UserInvitations.Add(invitation);
        await context.SaveChangesAsync();

        var command = new CompleteUserRegistrationCommand("used-token", "Password123!", "Password123!");

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task DeleteUserCommand_MainUserDeletingRegularUser_ShouldSucceed()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var handler = new DeleteUserCommandHandler(mockUserManager.Object);

        var mainUser = new ApplicationUser
        {
            Id = "main-user-id",
            Email = "main@example.com",
            IsMainUser = true
        };

        var regularUser = new ApplicationUser
        {
            Id = "regular-user-id",
            Email = "regular@example.com",
            IsMainUser = false
        };

        mockUserManager.Setup(m => m.FindByIdAsync("main-user-id"))
            .ReturnsAsync(mainUser);

        mockUserManager.Setup(m => m.FindByIdAsync("regular-user-id"))
            .ReturnsAsync(regularUser);

        mockUserManager.Setup(m => m.DeleteAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        var command = new DeleteUserCommand("regular-user-id", "main-user-id");

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        mockUserManager.Verify(m => m.DeleteAsync(It.Is<ApplicationUser>(u => u.Id == "regular-user-id")), Times.Once);
    }

    [Fact]
    public async Task DeleteUserCommand_NonMainUserDeleting_ShouldFail()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var handler = new DeleteUserCommandHandler(mockUserManager.Object);

        var regularUser = new ApplicationUser
        {
            Id = "regular-user-id",
            Email = "regular@example.com",
            IsMainUser = false
        };

        mockUserManager.Setup(m => m.FindByIdAsync("regular-user-id"))
            .ReturnsAsync(regularUser);

        var command = new DeleteUserCommand("some-user-id", "regular-user-id");

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task DeleteUserCommand_DeletingMainUser_ShouldFail()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var handler = new DeleteUserCommandHandler(mockUserManager.Object);

        var mainUser = new ApplicationUser
        {
            Id = "main-user-id",
            Email = "main@example.com",
            IsMainUser = true
        };

        mockUserManager.Setup(m => m.FindByIdAsync("main-user-id"))
            .ReturnsAsync(mainUser);

        var command = new DeleteUserCommand("main-user-id", "main-user-id");

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }
}
