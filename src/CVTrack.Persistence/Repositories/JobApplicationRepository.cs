using CVTrack.Application.Interfaces;
using CVTrack.Domain.Common;
using CVTrack.Domain.Entities;
using CVTrack.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace CVTrack.Persistence.Repositories;

public class JobApplicationRepository : IJobApplicationRepository
{
    private readonly AppDbContext _context;

    public JobApplicationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<JobApplication?> GetByIdAsync(Guid id)
    {
        return await _context.JobApplications
            .Where(j => j.Id == id && !j.IsDeleted)
            .Include(j => j.User)
            .Include(j => j.CV)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<JobApplication>> GetByUserIdAsync(Guid userId)
    {
        return await _context.JobApplications
            .Where(j => j.UserId == userId && !j.IsDeleted)
            .Include(j => j.CV)
            .ToListAsync();
    }

    public async Task<JobApplication> AddAsync(JobApplication application)
    {
        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync();
        return application;
    }

    public async Task UpdateAsync(JobApplication application)
    {
        _context.JobApplications.Update(application);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(JobApplication application)
    {
        _context.JobApplications.Remove(application);
        await _context.SaveChangesAsync();
    }

    // tüm verileri getir (soft delete durumu fark etmeksizin)
    public async Task<IEnumerable<JobApplication>> GetAllAsync()
    {
        return await _context.JobApplications
            .Include(j => j.CV)
            .Include(j => j.User)
            .ToListAsync(); // IsDeleted filtresi YOK
    }

    // sadece aktif verileri getir (IsDeleted = false)
    public async Task<PagedResult<JobApplication>> GetAllActiveJobApplicationsAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        ApplicationStatus? status = null)
    {
        var query = _context.JobApplications
            .Where(j => !j.IsDeleted) // Soft delete filtresi 
            .AsQueryable();

        // SearchTerm filtresi
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(j =>
                j.CompanyName.Contains(searchTerm) ||
                (j.Notes != null && j.Notes.Contains(searchTerm)));
        }

        // Status filtresi
        if (status.HasValue)
        {
            query = query.Where(j => j.Status == status.Value);
        }

        query = query
            .Include(j => j.User)
            .Include(j => j.CV);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(j => j.ApplicationDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<JobApplication>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    // tüm verileri getir - sayfalanmış (soft delete durumu fark etmeksizin)
    public async Task<PagedResult<JobApplication>> GetAllJobApplicationsPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        ApplicationStatus? status = null)
    {
        var query = _context.JobApplications
            .AsQueryable(); // IsDeleted filtresi yok yani herşey geliyor

        // SearchTerm filtresi
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(j =>
                j.CompanyName.Contains(searchTerm) ||
                (j.Notes != null && j.Notes.Contains(searchTerm)));
        }

        // Status filtresi
        if (status.HasValue)
        {
            query = query.Where(j => j.Status == status.Value);
        }

        query = query
            .Include(j => j.User)
            .Include(j => j.CV);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(j => j.ApplicationDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<JobApplication>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    // Kullanıcının kendi başvuruları için filtreleme (searchTerm ve status birlikte çalışabilir)
    public async Task<PagedResult<JobApplication>> GetJobApplicationsByUserFilteredPagedAsync(
        int pageNumber,
        int pageSize,
        Guid userId,
        string? searchTerm = null,
        ApplicationStatus? status = null)
    {
        var query = _context.JobApplications
            .Where(j => j.UserId == userId && !j.IsDeleted);

        // SearchTerm filtresi
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(j =>
                j.CompanyName.Contains(searchTerm) ||
                (j.Notes != null && j.Notes.Contains(searchTerm)));
        }

        // Status filtresi
        if (status.HasValue)
        {
            query = query.Where(j => j.Status == status.Value);
        }

        query = query.Include(j => j.User).Include(j => j.CV);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(j => j.ApplicationDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<JobApplication>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    // aktif veriler için filtreleme (admin kullanımı için)
    public async Task<PagedResult<JobApplication>> GetJobApplicationsFilteredPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        ApplicationStatus? status = null)
    {
        var query = _context.JobApplications
            .Where(j => !j.IsDeleted)
            .AsQueryable();

        // SearchTerm filtresi
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(j =>
                (j.User.FirstName != null && j.User.FirstName.Contains(searchTerm)) ||
                (j.User.LastName != null && j.User.LastName.Contains(searchTerm)) ||
                (j.CompanyName != null && j.CompanyName.Contains(searchTerm)) ||
                (j.Notes != null && j.Notes.Contains(searchTerm)));
        }

        // Status filtresi
        if (status.HasValue)
        {
            query = query.Where(j => j.Status == status.Value);
        }

        query = query
            .Include(j => j.User)
            .Include(j => j.CV);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(j => j.ApplicationDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<JobApplication>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    // eski metodlar - geriye dönük uyumluluk için tutuyorum ama artık kullanılmayacak
    public async Task<PagedResult<JobApplication>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await GetJobApplicationsFilteredPagedAsync(pageNumber, pageSize);
    }

    public async Task<PagedResult<JobApplication>> GetJobApplicationsByStatusPagedAsync(int pageNumber, int pageSize, ApplicationStatus status)
    {
        return await GetJobApplicationsFilteredPagedAsync(pageNumber, pageSize, null, status);
    }

    public async Task<PagedResult<JobApplication>> SearchJobApplicationsPagedAsync(int pageNumber, int pageSize, string searchTerm)
    {
        return await GetJobApplicationsFilteredPagedAsync(pageNumber, pageSize, searchTerm);
    }

    public async Task<PagedResult<JobApplication>> GetJobApplicationsByUserPagedAsync(int pageNumber, int pageSize, Guid userId)
    {
        return await GetJobApplicationsByUserFilteredPagedAsync(pageNumber, pageSize, userId);
    }
}