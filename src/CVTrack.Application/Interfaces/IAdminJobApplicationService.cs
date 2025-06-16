using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Application.JobApplications.Commands;
using CVTrack.Application.JobApplications.Queries;

namespace CVTrack.Application.Interfaces;

public interface IAdminJobApplicationService
{
    Task<IEnumerable<AdminJobApplicationDto>> GetAllAsync(GetAllJobApplicationsQuery query);
    Task UpdateStatusAsync(UpdateJobApplicationStatusCommand command);
    Task DeleteAsync(DeleteJobApplicationCommand command);
}

