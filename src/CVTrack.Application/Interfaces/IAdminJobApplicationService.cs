using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Application.JobApplications.Commands;
using CVTrack.Application.JobApplications.Queries;
using CVTrack.Domain.Common;
using CVTrack.Domain.Entities;

namespace CVTrack.Application.Interfaces;

public interface IAdminJobApplicationService
{
    Task<IEnumerable<AdminJobApplicationDto>> GetAllAsync(GetAllJobApplicationsQuery query);
    Task UpdateStatusAsync(UpdateJobApplicationStatusCommand command);
    Task DeleteAsync(DeleteJobApplicationCommand command);
    Task<PagedResult<AdminJobApplicationDto>> GetAllActivePagedJobApplications(
        GetAllJobApplicationsQuery query
    );
    Task<PagedResult<AdminJobApplicationDto>> GetAllPagedAsync(GetAllJobApplicationsQuery query);
}
