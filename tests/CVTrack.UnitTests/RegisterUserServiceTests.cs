using System;
using System.Threading.Tasks;
using CVTrack.Application.Users.Commands;
using CVTrack.Application.Users.Services;
using CVTrack.Application.Interfaces;
using CVTrack.Application.DTOs;
using CVTrack.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CVTrack.UnitTests;


public class RegisterUserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly RegisterUserService _service;

    public RegisterUserServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _service = new RegisterUserService(_userRepoMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_Should_Throw_When_Email_Exists()
    {
        // Arrange
        var cmd = new RegisterUserCommand
        {
            FirstName = "Test",
            LastName = "User",
            Email = "existing@example.com",
            Password = "Password123"
        };
        _userRepoMock.Setup(r => r.ExistsByEmailAsync(cmd.Email)).ReturnsAsync(true);

        // Act
        Func<Task> act = () => _service.RegisterAsync(cmd);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("User with this email already exists.");
    }

    [Fact]
    public async Task RegisterAsync_Should_Create_User_And_Return_UserDto()
    {
        // Arrange
        var cmd = new RegisterUserCommand
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            Password = "SecurePass"
        };
        _userRepoMock.Setup(r => r.ExistsByEmailAsync(cmd.Email)).ReturnsAsync(false);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                     .ReturnsAsync((User u) => u);

        // Act
        var result = await _service.RegisterAsync(cmd);

        // Assert
        _userRepoMock.Verify(r => r.AddAsync(
            It.Is<User>(u =>
                u.FirstName == cmd.FirstName &&
                u.LastName == cmd.LastName &&
                u.Email == cmd.Email &&
                !string.IsNullOrEmpty(u.PasswordHash)
            )
        ), Times.Once);

        result.Should().BeOfType<UserDto>();
        result.FirstName.Should().Be(cmd.FirstName);
        result.LastName.Should().Be(cmd.LastName);
        result.Email.Should().Be(cmd.Email);
        result.Id.Should().NotBeEmpty();
    }
}