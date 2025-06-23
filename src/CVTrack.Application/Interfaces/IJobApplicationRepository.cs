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
}