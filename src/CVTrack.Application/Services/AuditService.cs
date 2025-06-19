using System;
using System.Threading.Tasks;
using CVTrack.Application.Interfaces;
using CVTrack.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CVTrack.Application.Services;

public class AuditService : IAuditService
{
    private readonly IAuditRepository _auditRepo;
    private readonly ILogger<AuditService> _logger;

    public AuditService(
        IAuditRepository auditRepo,
        ILogger<AuditService> logger)
    {
        _auditRepo = auditRepo;
        _logger = logger;
    }

    public async Task LogDownloadAsync(Guid userId, Guid cvId)
    {
        var entry = new Audit
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CvId = cvId,
            Action = AuditAction.Downloaded,
            Timestamp = DateTime.UtcNow
        };
        await _auditRepo.AddAsync(entry);

        _logger.LogInformation(
            "User {UserId} downloaded CV {CvId} at {Timestamp}",
            userId, cvId, entry.Timestamp);
    }
}

