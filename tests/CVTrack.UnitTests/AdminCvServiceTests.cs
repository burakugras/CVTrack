using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CVTrack.Application.CVs.Commands;
using CVTrack.Application.CVs.Queries;
using CVTrack.Application.CVs.Services;
using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CVTrack.UnitTests;

public class AdminCvServiceTests
{
    private readonly Mock<ICVRepository> _repo;
    private readonly AdminCvService _service;

    public AdminCvServiceTests()
    {
        _repo = new Mock<ICVRepository>();
        _service = new AdminCvService(_repo.Object);
    }

    [Fact]
    public async Task GetAllAsync_Should_Map_All_CVs_To_AdminCvDto()
    {
        // Arrange
        var cvs = new List<CV>
            {
                new CV { Id=Guid.NewGuid(), UserId=Guid.NewGuid(), FileName="a.pdf", UploadDate=DateTime.UtcNow,
                         User=new User{FirstName="F1",LastName="L1",Email="e1"} },
                new CV { Id=Guid.NewGuid(), UserId=Guid.NewGuid(), FileName="b.pdf", UploadDate=DateTime.UtcNow,
                         User=new User{FirstName="F2",LastName="L2",Email="e2"} }
            };
        _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(cvs);

        // Act
        var dtos = await _service.GetAllAsync(new GetAllCvsQuery());

        // Assert
        dtos.Should().HaveCount(2);
        dtos.Select(d => d.UserFirstName).Should().BeEquivalentTo(new[] { "F1", "F2" });
    }

    [Fact]
    public async Task GetByIdAsync_Should_Map_Single_CV_To_AdminCvDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cv = new CV
        {
            Id = id,
            UserId = Guid.NewGuid(),
            FileName = "x.pdf",
            UploadDate = DateTime.UtcNow,
            User = new User { FirstName = "F", LastName = "L", Email = "e" }
        };
        _repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(cv);

        // Act
        var dto = await _service.GetByIdAsync(id);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(id);
        dto.UserFirstName.Should().Be("F");
    }

    [Fact]
    public async Task DeleteAsync_Should_SoftDelete_When_CV_Found()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cv = new CV { Id = id, UserId = Guid.NewGuid(), FileName = "x.pdf", UploadDate = DateTime.UtcNow, IsDeleted = false };
        _repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(cv);

        // Act
        await _service.DeleteAsync(id);

        // Assert
        _repo.Verify(r => r.UpdateAsync(cv), Times.Once);
        cv.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_Should_Throw_When_NotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((CV?)null);

        // Act
        Func<Task> act = () => _service.DeleteAsync(id);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
                 .WithMessage($"CV Id={id} bulunamadı.");
    }
}