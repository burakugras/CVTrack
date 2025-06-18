namespace CVTrack.Application.Interfaces;

public interface IAuditService
{
    Task LogDownloadAsync(Guid userId, Guid cvId);
}