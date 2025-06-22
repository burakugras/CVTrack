using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CVTrack.Application.JobApplications.Commands;
using CVTrack.Application.JobApplications.Services;
using CVTrack.Application.Interfaces;
using CVTrack.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CVTrack.UnitTests;

public class JobApplicationServiceTests
{
    private readonly Mock<IJobApplicationRepository> _jobRepoMock;
    private readonly JobApplicationService _service;

    public JobApplicationServiceTests()
    {
        _jobRepoMock = new Mock<IJobApplicationRepository>();
        _service = new JobApplicationService(_jobRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_Should_Call_AddAsync_And_Return_Entity()
    {
        // Arrange
        var cmd = new CreateJobApplicationCommand
        {
            UserId = Guid.NewGuid(),
            CVId = Guid.NewGuid(),
            CompanyName = "ACME",
            ApplicationDate = DateTime.UtcNow,
            Status = ApplicationStatus.Pending,
            Notes = "Test note"
        };
        _jobRepoMock.Setup(r => r.AddAsync(It.IsAny<JobApplication>()))
                    .ReturnsAsync((JobApplication j) => j);

        // Act
        var result = await _service.CreateAsync(cmd);

        // Assert
        _jobRepoMock.Verify(r => r.AddAsync(
            It.Is<JobApplication>(j =>
                j.UserId == cmd.UserId &&
                j.CVId == cmd.CVId &&
                j.CompanyName == cmd.CompanyName &&
                j.ApplicationDate == cmd.ApplicationDate &&
                j.Status == cmd.Status &&
                j.Notes == cmd.Notes)
        ), Times.Once);

        result.Should().BeOfType<JobApplication>();
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_When_Entity_Exists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existing = new JobApplication { Id = id, UserId = Guid.NewGuid(), CVId = Guid.NewGuid(), CompanyName = "Old", ApplicationDate = DateTime.UtcNow.AddDays(-1), Status = ApplicationStatus.Pending };
        _jobRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

        var cmd = new UpdateJobApplicationCommand
        {
            Id = id,
            UserId = existing.UserId,
            CVId = existing.CVId,
            CompanyName = "New",
            ApplicationDate = DateTime.UtcNow,
            Status = ApplicationStatus.Accepted,
            Notes = "Updated"
        };

        // Act
        Func<Task> act = () => _service.UpdateAsync(cmd);

        // Assert
        await act.Should().NotThrowAsync();
        _jobRepoMock.Verify(r => r.UpdateAsync(
            It.Is<JobApplication>(j =>
                j.Id == cmd.Id &&
                j.CompanyName == cmd.CompanyName &&
                j.Status == cmd.Status &&
                j.Notes == cmd.Notes
            )
        ), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Throw_When_NotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _jobRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((JobApplication?)null!);
        var cmd = new UpdateJobApplicationCommand { Id = id };

        // Act
        Func<Task> act = () => _service.UpdateAsync(cmd);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
                 .WithMessage($"Id={id} bulunamadı.");
    }

    [Fact]
    public async Task DeleteAsync_Should_SoftDelete_When_Entity_Exists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var jobApplication = new JobApplication { Id = id, UserId = Guid.NewGuid(), CVId = Guid.NewGuid(), CompanyName = "Test", IsDeleted = false };
        _jobRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(jobApplication);

        // Act
        await _service.DeleteAsync(id);

        // Assert
        _jobRepoMock.Verify(r => r.UpdateAsync(jobApplication), Times.Once);
        jobApplication.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_Should_Throw_When_NotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _jobRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((JobApplication?)null!);

        // Act
        Func<Task> act = () => _service.DeleteAsync(id);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
                 .WithMessage($"Id={id} bulunamadı.");
    }

    [Fact]
    public async Task GetByUserAsync_Should_Return_List()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var list = new List<JobApplication>
            {
                new JobApplication { Id = Guid.NewGuid(), UserId = userId, CVId = Guid.NewGuid() },
                new JobApplication { Id = Guid.NewGuid(), UserId = userId, CVId = Guid.NewGuid() }
            };
        _jobRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(list);

        // Act
        var result = await _service.GetByUserAsync(userId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(list);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Entity()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new JobApplication { Id = id };
        _jobRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().Be(entity);
    }
}