using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using Moq;
using CVTrack.Api.Controllers.Admin;
using CVTrack.Application.Users.Queries;
using CVTrack.Application.Users.Commands;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using CVTrack.Domain.Common;

namespace CVTrack.UnitTests.ControllerTests;


public class UsersAdminControllerTests
{
    private readonly Mock<IAdminUserService> _adminServiceMock;
    private readonly UsersAdminController _controller;

    public UsersAdminControllerTests()
    {
        _adminServiceMock = new Mock<IAdminUserService>();
        _controller = new UsersAdminController(_adminServiceMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task GetAllUsers_ReturnsOk_With_Paged_AdminUserDtoList()
    {
        // Arrange
        var dtos = new List<AdminUserDto>
            {
                new() { Id = Guid.NewGuid(), FirstName = "A", LastName = "B", Email = "a@b.com", Role = "User" }
            };
        var paged = new PagedResult<AdminUserDto>
        {
            Items = dtos,
            TotalCount = dtos.Count,
            PageNumber = 1,
            PageSize = 10
        };
        _adminServiceMock
            .Setup(s => s.GetAllPagedAsync(It.IsAny<GetAllUsersQuery>()))
            .ReturnsAsync(paged);

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        var body = ok!.Value as PagedResult<AdminUserDto>;
        body.Should().NotBeNull();
        body!.Items
            .Should().HaveCount(1)
            .And.ContainSingle(u => u.Email == "a@b.com");
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