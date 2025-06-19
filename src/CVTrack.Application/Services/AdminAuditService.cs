using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Application.Audits.Queries;
using CVTrack.Domain.Entities;

namespace CVTrack.Application.Services
{
    public class AdminAuditService : IAdminAuditService
    {
        private readonly IAuditRepository _auditRepo;

        public AdminAuditService(IAuditRepository auditRepo)
            => _auditRepo = auditRepo;

        public async Task<IEnumerable<AuditDto>> GetAllAsync(GetAuditsQuery query)
        {
            var entries = await _auditRepo.GetAllAsync();
            var list = entries
                // .Where(a => !query.CvId.HasValue || a.CvId == query.CvId)
                .Where(a => !query.UserId.HasValue || a.UserId == query.UserId)
                .Select(a => new AuditDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    CvId = a.CvId,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Action = a.Action.ToString(),
                    Timestamp = a.Timestamp
                });
            return list;
        }
    }
}
