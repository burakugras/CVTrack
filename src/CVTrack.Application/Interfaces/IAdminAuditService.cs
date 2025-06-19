using CVTrack.Application.DTOs;
using CVTrack.Application.Audits.Queries;

namespace CVTrack.Application.Interfaces;

public interface IAdminAuditService
{
    Task<IEnumerable<AuditDto>> GetAllAsync(GetAuditsQuery query);
}

