using CVTrack.Domain.Entities;

namespace CVTrack.Application.JobApplications.Commands;

public class UpdateJobApplicationStatusCommand
{
    public Guid Id { get; set; }
    public ApplicationStatus Status { get; set; }
}
