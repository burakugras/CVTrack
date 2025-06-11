namespace CVTrack.Domain.Entities;

public enum ApplicationStatus
{
    Pending,
    Accepted,
    Rejected
}

public class JobApplication
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CVId { get; set; }

    public string CompanyName { get; set; } = null!;
    public DateTime ApplicationDate { get; set; }
    public ApplicationStatus Status { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public CV CV { get; set; } = null!;
}