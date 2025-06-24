using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Application.JobApplications.Commands;
using CVTrack.Application.JobApplications.Queries;
using CVTrack.Application.JobApplications.Services;
using CVTrack.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CVTrack.UnitTests;

public class AdminJobApplicationServiceTests
{
    private readonly Mock<IJobApplicationRepository> _repo;
    private readonly AdminJobApplicationService _service;

    public AdminJobApplicationServiceTests()
    {
        _repo = new Mock<IJobApplicationRepository>();
        _service = new AdminJobApplicationService(_repo.Object);
    }

    [Fact]
    public async Task GetAllAsync_Should_Map_All_Applications_To_Dto()
    {
        // Arrange
        var list = new List<JobApplication>
            {
                new JobApplication { Id=Guid.NewGuid(), UserId=Guid.NewGuid(), CVId=Guid.NewGuid(), CompanyName="C1", ApplicationDate=DateTime.UtcNow, Status=ApplicationStatus.Pending },
                new JobApplication { Id=Guid.NewGuid(), UserId=Guid.NewGuid(), CVId=Guid.NewGuid(), CompanyName="C2", ApplicationDate=DateTime.UtcNow, Status=ApplicationStatus.Accepted }
            };
        _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

        // Act
        var dtos = await _service.GetAllAsync(new GetAllJobApplicationsQuery());

        // Assert
        dtos.Should().HaveCount(2);
        dtos.Select(d => d.Id).Should().BeEquivalentTo(list.Select(a => a.Id));
        dtos.Should().Contain(d => d.CompanyName == "C2" && d.Status == ApplicationStatus.Accepted);
    }

    [Fact]
    public async Task UpdateStatusAsync_Should_Update_Status_When_Exists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new JobApplication { Id = id, Status = ApplicationStatus.Rejected };
        _repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

        var cmd = new UpdateJobApplicationStatusCommand { Id = id, Status = ApplicationStatus.Rejected };

        // Act
        await _service.UpdateStatusAsync(cmd);

        // Assert
        _repo.Verify(r => r.UpdateAsync(It.Is<JobApplication>(j =>
            j.Id == id && j.Status == ApplicationStatus.Rejected)), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_Should_Throw_When_NotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((JobApplication?)null!);
        var cmd = new UpdateJobApplicationStatusCommand { Id = id, Status = ApplicationStatus.Pending };

        // Act
        Func<Task> act = () => _service.UpdateStatusAsync(cmd);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
                 .WithMessage($"Id={id} bulunamadı.");
    }

    [Fact]
    public async Task DeleteAsync_Should_Call_UpdateAsync_When_Exists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new JobApplication { Id = id };
        _repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

        // Act
        await _service.DeleteAsync(new DeleteJobApplicationCommand { Id = id });

        // Assert
        _repo.Verify(r => r.UpdateAsync(It.Is<JobApplication>(j => j.Id == id)), Times.Once);
    }
}

