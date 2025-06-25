using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CVTrack.Application.Audits.Queries;
using CVTrack.Application.Audits.Services;

// using CVTrack.Application.Services;
using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CVTrack.UnitTests;

public class AdminAuditServiceTests
{
    private readonly Mock<IAuditRepository> _repo;
    private readonly AdminAuditService _service;

    public AdminAuditServiceTests()
    {
        _repo = new Mock<IAuditRepository>();
        _service = new AdminAuditService(_repo.Object);
    }

    [Fact]
    public async Task GetAllAsync_Should_Map_All_Audits_To_Dto()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var entries = new List<Audit>
            {
                new Audit { Id=Guid.NewGuid(), UserId=Guid.NewGuid(), CvId=Guid.NewGuid(), Action=AuditAction.Downloaded, Timestamp=now },
                new Audit { Id=Guid.NewGuid(), UserId=Guid.NewGuid(), CvId=Guid.NewGuid(), Action=AuditAction.Downloaded, Timestamp=now }
            };
        _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(entries);

        // Act
        var dtos = await _service.GetAllAsync(new GetAuditsQuery());

        // Assert
        dtos.Should().HaveCount(2);
        dtos.Select(d => d.CvId).Should().BeEquivalentTo(entries.Select(e => e.CvId));
    }

    [Fact]
    public async Task GetAllAsync_Should_Filter_By_UserId()
    {
        // Arrange
        var targetUser = Guid.NewGuid();
        var entries = new List<Audit>
            {
                new Audit { Id=Guid.NewGuid(), UserId=targetUser, CvId=Guid.NewGuid(), Action=AuditAction.Downloaded, Timestamp=DateTime.UtcNow },
                new Audit { Id=Guid.NewGuid(), UserId=Guid.NewGuid(), CvId=Guid.NewGuid(), Action=AuditAction.Downloaded, Timestamp=DateTime.UtcNow }
            };
        _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(entries);

        // Act
        var dtos = await _service.GetAllAsync(new GetAuditsQuery { UserId = targetUser });

        // Assert
        dtos.Should().HaveCount(1);
        dtos.All(d => d.UserId == targetUser).Should().BeTrue();
    }
}

