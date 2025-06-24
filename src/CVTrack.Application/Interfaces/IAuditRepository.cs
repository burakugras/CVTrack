using CVTrack.Domain.Entities;

namespace CVTrack.Application.Interfaces;

public interface IAuditRepository
{
    Task AddAsync(Audit audit);
    Task<IEnumerable<Audit>> GetAllAsync();
    //İleride : Task<IEnumerable<Audit>> GetByCvIdAsync(Guid cvId);
}
