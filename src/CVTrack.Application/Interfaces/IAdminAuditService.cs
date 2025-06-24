using CVTrack.Application.Audits.Queries;
using CVTrack.Application.DTOs;

namespace CVTrack.Application.Interfaces;

public interface IAdminAuditService
{
    Task<IEnumerable<AuditDto>> GetAllAsync(GetAuditsQuery query);
    // Task<IEnumerable<AuditDto>> GetAllAuditsAsync();
}
