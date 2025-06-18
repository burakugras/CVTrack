using CVTrack.Domain.Entities;

namespace CVTrack.Application.Interfaces;

public interface IAuditRepository
{
    Task AddAsync(Audit audit);
    //İleride : Task<IEnumerable<Audit>> GetByCvIdAsync(Guid cvId);


}