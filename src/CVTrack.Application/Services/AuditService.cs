using System;
using System.Threading.Tasks;
using CVTrack.Application.Interfaces;
using CVTrack.Domain.Entities;

namespace CVTrack.Application.Services;

public class AuditService : IAuditService
{
    private readonly IAuditRepository _auditRepo;

    public AuditService(
        IAuditRepository auditRepo)
    {
        _auditRepo = auditRepo;
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
    }
}

