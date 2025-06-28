using CVTrack.Domain.Entities;

namespace CVTrack.Application.JobApplications.Queries;

public class GetAllJobApplicationsQuery
{
    public Guid? UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public ApplicationStatus? Status { get; set; }

}
