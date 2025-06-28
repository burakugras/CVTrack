using CVTrack.Domain.Common;
using CVTrack.Domain.Entities;

namespace CVTrack.Application.Interfaces;

public interface IJobApplicationRepository
{
    Task<JobApplication?> GetByIdAsync(Guid id);
    Task<IEnumerable<JobApplication>> GetByUserIdAsync(Guid userId);
    Task<JobApplication> AddAsync(JobApplication application);
    Task UpdateAsync(JobApplication application);
    Task RemoveAsync(JobApplication application);
    Task<IEnumerable<JobApplication>> GetAllAsync();
    Task<IEnumerable<JobApplication>> GetAllActiveJobApplicationsAsync();

    // Yeni filtreleme metodları
    Task<PagedResult<JobApplication>> GetJobApplicationsByUserFilteredPagedAsync(
        int pageNumber,
        int pageSize,
        Guid userId,
        string? searchTerm = null,
        ApplicationStatus? status = null);

    Task<PagedResult<JobApplication>> GetJobApplicationsFilteredPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        ApplicationStatus? status = null);

    // Eski metodlar - geriye dönük uyumluluk için
    Task<PagedResult<JobApplication>> GetPagedAsync(int pageNumber, int pageSize);
    Task<PagedResult<JobApplication>> GetJobApplicationsByStatusPagedAsync(int pageNumber, int pageSize, ApplicationStatus status);
    Task<PagedResult<JobApplication>> SearchJobApplicationsPagedAsync(int pageNumber, int pageSize, string searchTerm);
    Task<PagedResult<JobApplication>> GetJobApplicationsByUserPagedAsync(int pageNumber, int pageSize, Guid userId);
}