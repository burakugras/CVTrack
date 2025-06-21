namespace CVTrack.Domain.Entities;

public class Audit
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CvId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public AuditAction Action { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}