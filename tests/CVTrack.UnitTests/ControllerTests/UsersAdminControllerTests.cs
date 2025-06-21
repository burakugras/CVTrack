using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using Moq;
using CVTrack.Api.Controllers.Admin;
using CVTrack.Application.Users.Queries;
using CVTrack.Application.Users.Commands;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;

namespace CVTrack.UnitTests.ControllerTests;


public class UsersAdminControllerTests
{
    private readonly Mock<IAdminUserService> _adminServiceMock;
    private readonly UsersAdminController _controller;

    public UsersAdminControllerTests()
    {
        _adminServiceMock = new Mock<IAdminUserService>();
        _controller = new UsersAdminController(_adminServiceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_With_AdminUserDtoList()
    {
        // Arrange
        var dtos = new List<AdminUserDto> {
                new() { Id = Guid.NewGuid(), FirstName="A", LastName="B", Email="a@b.com", Role="User" }
            };
        _adminServiceMock
            .Setup(s => s.GetAllAsync(It.IsAny<GetAllUsersQuery>()))
            .ReturnsAsync(dtos);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeAssignableTo<IEnumerable<AdminUserDto>>()
           .Which.Should().HaveCount(1)
                    .And.ContainSingle(d => d.Email == "a@b.com");
    }

    [Fact]
    public async Task UpdateRole_IdMismatch_ReturnsBadRequest()
    {
        // Arrange
        var cmd = new UpdateUserRoleCommand { UserId = Guid.NewGuid(), NewRole = "Admin" };
        var wrongId = Guid.NewGuid();

        // Act
        var result = await _controller.UpdateRole(wrongId, cmd);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateRole_Valid_CallsServiceAndReturnsNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cmd = new UpdateUserRoleCommand { UserId = id, NewRole = "Admin" };

        // Act
        var result = await _controller.UpdateRole(id, cmd);

        // Assert
        _adminServiceMock.Verify(s => s.UpdateRoleAsync(cmd), Times.Once);
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_CallsServiceAndReturnsNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = await _controller.Delete(id);

        // Assert
        _adminServiceMock.Verify(s => s.DeleteAsync(
            It.Is<DeleteUserCommand>(c => c.UserId == id)), Times.Once);
        result.Should().BeOfType<NoContentResult>();
    }
}