using CVTrack.Domain.Entities;

namespace CVTrack.Application.DTOs;

public class AdminJobApplicationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CVId { get; set; }

    public string UserFirstName { get; set; } = null!;
    public string UserLastName { get; set; } = null!;
    public string UserMail { get; set; } = null!;
    public string? FileName { get; set; }

    public string CompanyName { get; set; } = null!;
    public DateTime ApplicationDate { get; set; }
    public ApplicationStatus Status { get; set; }
    public string? Notes { get; set; }
}

