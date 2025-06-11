using CVTrack.Domain.Entities;

namespace CVTrack.Application.JobApplications.Commands;

public class UpdateJobApplicationCommand
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CVId { get; set; }
    public string CompanyName { get; set; } = null!;
    public DateTime ApplicationDate { get; set; }
    public ApplicationStatus Status { get; set; }
    public string? Notes { get; set; }
}