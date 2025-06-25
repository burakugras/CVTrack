using CVTrack.Application.Interfaces;
using CVTrack.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CVTrack.Application.Audits.Services;

public class AuditService : IAuditService
{
    private readonly IAuditRepository _auditRepo;
    private readonly ILogger<AuditService> _logger;
    private readonly IUserRepository _userRepo;

    public AuditService(
        IAuditRepository auditRepo,
        ILogger<AuditService> logger,
        IUserRepository userRepo
        )
    {
        _auditRepo = auditRepo;
        _logger = logger;
        _userRepo = userRepo;
    }

    public async Task LogDownloadAsync(Guid userId, Guid cvId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        var entry = new Audit
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CvId = cvId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Action = AuditAction.Downloaded,
            Timestamp = DateTime.UtcNow
        };

        await _auditRepo.AddAsync(entry);

        _logger.LogInformation(
            "User {UserId} downloaded CV {CvId} at {Timestamp}",
            userId, cvId, entry.Timestamp);
    }
}

