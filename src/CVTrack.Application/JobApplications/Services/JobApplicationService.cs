using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Application.JobApplications.Commands;
using CVTrack.Application.JobApplications.Queries;
using CVTrack.Domain.Common;
using CVTrack.Domain.Entities;

namespace CVTrack.Application.JobApplications.Services;

public class JobApplicationService : IJobApplicationService
{
    private readonly IJobApplicationRepository _jobRepo;

    public JobApplicationService(IJobApplicationRepository jobRepo)
    {
        _jobRepo = jobRepo;
    }

    public async Task<JobApplication> CreateAsync(CreateJobApplicationCommand cmd)
    {
        var job = new JobApplication
        {
            Id = Guid.NewGuid(),
            UserId = cmd.UserId,
            CVId = cmd.CVId,
            CompanyName = cmd.CompanyName,
            ApplicationDate = cmd.ApplicationDate,
            Status = cmd.Status,
            Notes = cmd.Notes
        };

        return await _jobRepo.AddAsync(job);
    }

    public async Task UpdateAsync(UpdateJobApplicationCommand cmd)
    {
        var existing = await _jobRepo.GetByIdAsync(cmd.Id)
                      ?? throw new KeyNotFoundException($"Id={cmd.Id} bulunamadı.");

        existing.CompanyName = cmd.CompanyName;
        existing.ApplicationDate = cmd.ApplicationDate;
        existing.Status = cmd.Status;
        existing.Notes = cmd.Notes;

        await _jobRepo.UpdateAsync(existing);
    }

    public async Task DeleteAsync(Guid id)
    {
        var existing = await _jobRepo.GetByIdAsync(id)
                      ?? throw new KeyNotFoundException($"Id={id} bulunamadı.");

        existing.IsDeleted = true;
        await _jobRepo.UpdateAsync(existing);
    }

    public async Task<IEnumerable<JobApplication>> GetByUserAsync(Guid userId)
        => await _jobRepo.GetByUserIdAsync(userId);

    public async Task<JobApplication?> GetByIdAsync(Guid id)
        => await _jobRepo.GetByIdAsync(id);

    public async Task<PagedResult<JobApplicationDto>> GetAllPagedAsync(GetAllJobApplicationsQuery query)
    {
        var pagination = new PaginationRequest
        {
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };

        PagedResult<JobApplication> pagedJobApplications;

        // UserId varsa (kullanıcının kendi başvurularını getiriyor)
        if (query.UserId.HasValue)
        {
            pagedJobApplications = await _jobRepo.GetJobApplicationsByUserFilteredPagedAsync(
                pagination.ValidatedPageNumber,
                pagination.ValidatedPageSize,
                query.UserId.Value,
                query.SearchTerm,
                query.Status
            );
        }
        else
        {
            // Admin için tüm başvuruları getir (bu durumda muhtemelen hiç olmayacak ama güvenlik için)
            pagedJobApplications = await _jobRepo.GetJobApplicationsFilteredPagedAsync(
                pagination.ValidatedPageNumber,
                pagination.ValidatedPageSize,
                query.SearchTerm,
                query.Status
            );
        }

        // JobApplication'ları DTO'ya dönüştür
        var jobApplicationDtos = pagedJobApplications.Items.Select(u => new JobApplicationDto
        {
            Id = u.Id,
            UserId = u.UserId,
            CVId = u.CVId,
            CompanyName = u.CompanyName,
            ApplicationDate = u.ApplicationDate,
            Status = u.Status,
            Notes = u.Notes
        });

        return new PagedResult<JobApplicationDto>
        {
            Items = jobApplicationDtos,
            TotalCount = pagedJobApplications.TotalCount,
            PageNumber = pagedJobApplications.PageNumber,
            PageSize = pagedJobApplications.PageSize
        };
    }
}