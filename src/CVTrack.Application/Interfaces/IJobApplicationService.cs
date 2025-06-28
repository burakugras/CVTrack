using CVTrack.Application.DTOs;
using CVTrack.Application.JobApplications.Commands;
using CVTrack.Application.JobApplications.Queries;
using CVTrack.Domain.Common;
using CVTrack.Domain.Entities;

namespace CVTrack.Application.Interfaces;

public interface IJobApplicationService
{
    Task<JobApplication> CreateAsync(CreateJobApplicationCommand command);
    Task UpdateAsync(UpdateJobApplicationCommand command);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<JobApplication>> GetByUserAsync(Guid userId);
    Task<JobApplication?> GetByIdAsync(Guid id);
    Task<PagedResult<JobApplicationDto>> GetAllPagedAsync(GetAllJobApplicationsQuery query);
}