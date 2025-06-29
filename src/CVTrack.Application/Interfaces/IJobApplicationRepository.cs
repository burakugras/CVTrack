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

    // tüm verileri getir (soft delete durumu fark etmeksizin)
    Task<IEnumerable<JobApplication>> GetAllAsync();

    // sadece aktif verileri getir (IsDeleted = false)
    Task<PagedResult<JobApplication>> GetAllActiveJobApplicationsAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        ApplicationStatus? status = null);

    // tüm verileri getir - sayfalanmış (soft delete durumu fark etmeksizin)
    Task<PagedResult<JobApplication>> GetAllJobApplicationsPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        ApplicationStatus? status = null);

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