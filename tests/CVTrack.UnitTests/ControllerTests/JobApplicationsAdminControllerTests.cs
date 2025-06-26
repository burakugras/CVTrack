using CVTrack.Api.Controllers.Admin;
using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Application.JobApplications.Commands;
using CVTrack.Application.JobApplications.Queries;
using CVTrack.Domain.Common;
using CVTrack.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CVTrack.UnitTests.ControllerTests;

public class JobApplicationsAdminControllerTests
{
    private readonly Mock<IAdminJobApplicationService> _adminServiceMock;
    private readonly JobApplicationsAdminController _controller;

    public JobApplicationsAdminControllerTests()
    {
        _adminServiceMock = new Mock<IAdminJobApplicationService>();
        _controller = new JobApplicationsAdminController(_adminServiceMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOk_With_Paged_AdminJobApplicationDtoList()
    {
        // Arrange
        var list = new List<AdminJobApplicationDto>
            {
                new() { Id = Guid.NewGuid(), CompanyName = "X" }
            };
        var paged = new PagedResult<AdminJobApplicationDto>
        {
            Items = list,
            TotalCount = list.Count,
            PageNumber = 1,
            PageSize = 10
        };
        _adminServiceMock
            .Setup(s => s.GetAllPagedAsync(It.IsAny<GetAllJobApplicationsQuery>()))
            .ReturnsAsync(paged);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        var body = ok!.Value as PagedResult<AdminJobApplicationDto>;
        body.Should().NotBeNull();
        body!.Items
            .Should().ContainSingle(d => d.CompanyName == "X");
    }

    [Fact]
    public async Task UpdateStatus_IdMismatch_ReturnsBadRequest()
    {
        // Arrange
        var cmd = new UpdateJobApplicationStatusCommand
        {
            Id = Guid.NewGuid(),
            Status = ApplicationStatus.Pending
        };
        var wrongId = Guid.NewGuid();

        // Act
        var result = await _controller.UpdateStatus(wrongId, cmd);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateStatus_Valid_CallsServiceAndReturnsNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cmd = new UpdateJobApplicationStatusCommand { Id = id, Status = ApplicationStatus.Accepted };

        // Act
        var result = await _controller.UpdateStatus(id, cmd);

        // Assert
        _adminServiceMock.Verify(s => s.UpdateStatusAsync(cmd), Times.Once);
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
            It.Is<DeleteJobApplicationCommand>(c => c.Id == id)), Times.Once);
        result.Should().BeOfType<NoContentResult>();
    }
}