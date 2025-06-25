using CVTrack.Application.Audits.Queries;
using CVTrack.Application.DTOs;
using CVTrack.Domain.Common;

namespace CVTrack.Application.Interfaces;

public interface IAdminAuditService
{
    Task<IEnumerable<AuditDto>> GetAllAsync(GetAuditsQuery query);
    // Task<IEnumerable<AuditDto>> GetAllAuditsAsync();
    Task<PagedResult<AuditDto>> GetAllPagedAsync(GetAllAuditsQuery query);
}
