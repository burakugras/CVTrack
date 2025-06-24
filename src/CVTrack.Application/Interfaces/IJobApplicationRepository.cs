using CVTrack.Domain.Common;
using CVTrack.Domain.Entities;

namespace CVTrack.Application.Interfaces;

public interface IJobApplicationRepository
{
    Task<JobApplication> GetByIdAsync(Guid id);
    Task<IEnumerable<JobApplication>> GetByUserIdAsync(Guid userId);
    Task<JobApplication> AddAsync(JobApplication application);
    Task UpdateAsync(JobApplication jobApplication);
    Task RemoveAsync(JobApplication jobApplication);
    Task<IEnumerable<JobApplication>> GetAllAsync();
    Task<IEnumerable<JobApplication>> GetAllActiveJobApplicationsAsync();
    Task<PagedResult<JobApplication>> GetPagedAsync(int pageNumber, int pageSize);
    Task<PagedResult<JobApplication>> GetJobApplicationsByStatusPagedAsync(
        int pageNumber,
        int pageSize,
        ApplicationStatus status
    );
    Task<PagedResult<JobApplication>> SearchJobApplicationsPagedAsync(
        int pageNumber,
        int pageSize,
        string searchTerm
    );
}
