using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Application.Users.Commands;
using CVTrack.Application.Users.Queries;
using CVTrack.Application.Users.Services;
using CVTrack.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CVTrack.UnitTests;

public class AdminUserServiceTests
{
    private readonly Mock<IUserRepository> _userRepo;
    private readonly AdminUserService _service;

    public AdminUserServiceTests()
    {
        _userRepo = new Mock<IUserRepository>();
        _service = new AdminUserService(_userRepo.Object);
    }

    [Fact]
    public async Task GetAllAsync_Should_Map_All_Users_To_AdminUserDto()
    {
        // Arrange
        var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), FirstName="A", LastName="AA", Email="a@x.com", Role=UserRole.User },
                new User { Id = Guid.NewGuid(), FirstName="B", LastName="BB", Email="b@x.com", Role=UserRole.Admin }
            };
        _userRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        // Act
        var dtos = await _service.GetAllAsync(new GetAllUsersQuery());

        // Assert
        dtos.Should().HaveCount(2);
        dtos.Select(d => d.Id).Should().BeEquivalentTo(users.Select(u => u.Id));
        dtos.Should().Contain(d => d.Role == "Admin" && d.Email == "b@x.com");
    }

    [Fact]
    public async Task UpdateRoleAsync_Should_Update_UserRole_When_Valid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = new User { Id = id, FirstName = "X", LastName = "Y", Email = "x@y.com", Role = UserRole.User };
        _userRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);

        var cmd = new UpdateUserRoleCommand { UserId = id, NewRole = "Admin" };

        // Act
        await _service.UpdateRoleAsync(cmd);

        // Assert
        _userRepo.Verify(r => r.UpdateAsync(It.Is<User>(u =>
            u.Id == id && u.Role == UserRole.Admin)), Times.Once);
    }

    [Fact]
    public async Task UpdateRoleAsync_Should_Throw_When_User_NotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _userRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((User?)null);
        var cmd = new UpdateUserRoleCommand { UserId = id, NewRole = "Admin" };

        // Act
        Func<Task> act = () => _service.UpdateRoleAsync(cmd);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
                 .WithMessage($"User Id={id} bulunamadı.");
    }

    [Fact]
    public async Task DeleteAsync_Should_Call_RemoveAsync_When_User_Exists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = new User { Id = id, FirstName = "A", LastName = "B", Email = "a@b.com" };
        _userRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);

        // Act
        await _service.DeleteAsync(new DeleteUserCommand { UserId = id });

        // Assert
        _userRepo.Verify(r => r.RemoveAsync(user), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Throw_When_User_NotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _userRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((User?)null);

        // Act
        Func<Task> act = () => _service.DeleteAsync(new DeleteUserCommand { UserId = id });

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
                 .WithMessage($"User Id={id} bulunamadı.");
    }
}

