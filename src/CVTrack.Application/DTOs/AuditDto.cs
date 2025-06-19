namespace CVTrack.Application.DTOs;

public class AuditDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CvId { get; set; }
    public string Action { get; set; } = null!;
    public DateTime Timestamp { get; set; }
}

