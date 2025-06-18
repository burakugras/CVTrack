namespace CVTrack.Domain.Entities;

public enum AuditAction
{
    Downloaded
    // ileride Uploaded, Deleted, vs. eklenebilir
}

public class Audit
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CvId { get; set; }
    public AuditAction Action { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}