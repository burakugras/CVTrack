using CVTrack.Domain.Common;
using CVTrack.Domain.Entities;

namespace CVTrack.Application.Interfaces;

public interface IAuditRepository
{
    Task AddAsync(Audit audit);
    Task<IEnumerable<Audit>> GetAllAsync();
    Task<PagedResult<Audit>> GetAuditsByUserPagedAsync(int pageNumber, int pageSize, Guid userId);

    //İleride : Task<IEnumerable<Audit>> GetByCvIdAsync(Guid cvId);
    Task<PagedResult<Audit>> GetPagedAsync(int pageNumber, int pageSize);
    Task<PagedResult<Audit>> GetAuditsByActionPagedAsync(int pageNumber, int pageSize, AuditAction action);
    Task<PagedResult<Audit>> SearchAuditsPagedAsync(int pageNumber, int pageSize, string searchTerm);
}
