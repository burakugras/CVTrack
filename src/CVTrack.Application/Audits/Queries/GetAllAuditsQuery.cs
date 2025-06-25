using CVTrack.Domain.Entities;

namespace CVTrack.Application.Audits.Queries;

public class GetAllAuditsQuery
{
    public Guid? UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public AuditAction? Action { get; set; }
}