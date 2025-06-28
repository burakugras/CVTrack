using System.Security.Claims;
using CVTrack.Api.Controllers.Admin;
using CVTrack.Application.CVs.Queries;
using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Domain.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CVTrack.UnitTests.ControllerTests;

public class CvsAdminControllerTests
{
    private readonly Mock<IAdminCvService> _cvServiceMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly CvsAdminController _controller;

    public CvsAdminControllerTests()
    {
        _cvServiceMock = new Mock<IAdminCvService>();
        _fileServiceMock = new Mock<IFileService>();
        _auditServiceMock = new Mock<IAuditService>();

        _controller = new CvsAdminController(
            _cvServiceMock.Object,
            _fileServiceMock.Object,
            _auditServiceMock.Object);

        // Fake HttpContext.User
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "TestAuth"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOk_With_PagedList()
    {
        // Arrange
        var dtoList = new List<AdminCvDto> {
        new() { Id = Guid.NewGuid(), FileName = "a.pdf" }
    };
        var pagedResult = new PagedResult<AdminCvDto>
        {
            Items = dtoList,
            TotalCount = dtoList.Count,
            PageNumber = 1,
            PageSize = 10
        };
        _cvServiceMock
            .Setup(s => s.GetAllPagedAsync(It.IsAny<GetAllCvsQuery>()))
            .ReturnsAsync(pagedResult);

        // Act
        var actionResult = await _controller.GetAll();

        // Assert
        var ok = actionResult.Result as OkObjectResult;
        ok.Should().NotBeNull();

        var returned = ok!.Value as PagedResult<AdminCvDto>;
        returned.Should().NotBeNull();
        returned!.Items
            .Should().HaveCount(1)
            .And.ContainSingle(c => c.FileName == "a.pdf");
        returned.TotalCount.Should().Be(1);
        returned.PageNumber.Should().Be(1);
        returned.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Download_ValidId_CallsAudit_AndReturnsFile()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new AdminCvDto { Id = id, FileName = "x.pdf" };
        var content = new byte[] { 1, 2, 3 };
        _cvServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(dto);
        _fileServiceMock.Setup(f => f.GetFileAsync("x.pdf"))
            .ReturnsAsync(content);

        // Act
        var result = await _controller.Download(id);

        // Assert
        _auditServiceMock.Verify(a => a.LogDownloadAsync(
            It.IsAny<Guid>(), id), Times.Once);

        var fileResult = result as FileContentResult;
        fileResult.Should().NotBeNull();
        fileResult!.FileContents.Should().Equal(content);
        fileResult.ContentType.Should().Be("application/pdf");
        fileResult.FileDownloadName.Should().Be("x.pdf");
    }

    [Fact]
    public async Task Download_NotFound_Returns404()
    {
        // Arrange
        var id = Guid.NewGuid();
        _cvServiceMock.Setup(s => s.GetByIdAsync(id))
                      .ReturnsAsync(new AdminCvDto { Id = id, FileName = "nofile.pdf" });
        _fileServiceMock.Setup(f => f.GetFileAsync("nofile.pdf"))
            .Throws<FileNotFoundException>();

        // Act
        var result = await _controller.Download(id);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }
}