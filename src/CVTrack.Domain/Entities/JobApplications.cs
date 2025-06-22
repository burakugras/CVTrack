namespace CVTrack.Domain.Entities;

public class JobApplication
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CVId { get; set; }

    public string CompanyName { get; set; } = null!;
    public DateTime ApplicationDate { get; set; }
    public ApplicationStatus Status { get; set; }
    public string? Notes { get; set; }

    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    public User User { get; set; } = null!;
    public CV CV { get; set; } = null!;
}