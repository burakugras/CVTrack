using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Application.JobApplications.Commands;
using CVTrack.Application.JobApplications.Queries;
using CVTrack.Domain.Common;
using CVTrack.Domain.Entities;

namespace CVTrack.Application.JobApplications.Services;

public class AdminJobApplicationService : IAdminJobApplicationService
{
    private readonly IJobApplicationRepository _repo;

    public AdminJobApplicationService(IJobApplicationRepository repo)
        => _repo = repo;

    public async Task<IEnumerable<AdminJobApplicationDto>> GetAllAsync(GetAllJobApplicationsQuery _)
    {
        var apps = await _repo.GetAllAsync();
        return apps.Select(j => new AdminJobApplicationDto
        {
            Id = j.Id,
            UserId = j.UserId,
            // j.User null ise boş string dönecek
            UserFirstName = j.User?.FirstName ?? string.Empty,
            UserLastName = j.User?.LastName ?? string.Empty,
            UserMail = j.User?.Email ?? string.Empty,
            CVId = j.CVId,
            CompanyName = j.CompanyName,
            ApplicationDate = j.ApplicationDate,
            Status = j.Status,
            Notes = j.Notes
        });
    }

    public async Task UpdateStatusAsync(UpdateJobApplicationStatusCommand cmd)
    {
        var j = await _repo.GetByIdAsync(cmd.Id)
              ?? throw new KeyNotFoundException($"Id={cmd.Id} bulunamadı.");

        j.Status = cmd.Status;
        await _repo.UpdateAsync(j);
    }

    public async Task DeleteAsync(DeleteJobApplicationCommand cmd)
    {
        var j = await _repo.GetByIdAsync(cmd.Id)
              ?? throw new KeyNotFoundException($"Id={cmd.Id} bulunamadı.");

        j.IsDeleted = true;

        await _repo.UpdateAsync(j);
    }

    public async Task<IEnumerable<AdminJobApplicationDto>> GetAllActiveJobApplications(GetAllJobApplicationsQuery _)
    {
        var apps = await _repo.GetAllActiveJobApplicationsAsync();
        return apps.Select(j => new AdminJobApplicationDto
        {
            Id = j.Id,
            UserId = j.UserId,
            UserFirstName = j.User?.FirstName ?? string.Empty,
            UserLastName = j.User?.LastName ?? string.Empty,
            UserMail = j.User?.Email ?? string.Empty,
            CVId = j.CVId,
            CompanyName = j.CompanyName,
            ApplicationDate = j.ApplicationDate,
            Status = j.Status,
            Notes = j.Notes
        });
    }

    public async Task<PagedResult<AdminJobApplicationDto>> GetAllPagedAsync(GetAllJobApplicationsQuery query)
    {
        var pagination = new PaginationRequest
        {
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };

        PagedResult<JobApplication> pagedJobApplications;

        // Search varsa
        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            pagedJobApplications = await _repo.SearchJobApplicationsPagedAsync(
                pagination.ValidatedPageNumber,
                pagination.ValidatedPageSize,
                query.SearchTerm
            );
        }
        // Role filter varsa
        else if (query.Status.HasValue)
        {
            pagedJobApplications = await _repo.GetJobApplicationsByStatusPagedAsync(
                pagination.ValidatedPageNumber,
                pagination.ValidatedPageSize,
                query.Status.Value
            );
        }
        // Normal pagination
        else
        {
            pagedJobApplications = await _repo.GetPagedAsync(
                pagination.ValidatedPageNumber,
                pagination.ValidatedPageSize
            );
        }

        // User'ları AdminUserDto'ya dönüştür
        var adminJobApplicationDtos = pagedJobApplications.Items.Select(u => new AdminJobApplicationDto
        {
            Id = u.Id,
            UserId = u.UserId,
            UserFirstName = u.User?.FirstName ?? string.Empty,
            UserLastName = u.User?.LastName ?? string.Empty,
            UserMail = u.User?.Email ?? string.Empty,
            CVId = u.CVId,
            CompanyName = u.CompanyName,
            ApplicationDate = u.ApplicationDate,
            Status = u.Status,
            Notes = u.Notes
        });

        return new PagedResult<AdminJobApplicationDto>
        {
            Items = adminJobApplicationDtos,
            TotalCount = pagedJobApplications.TotalCount,
            PageNumber = pagedJobApplications.PageNumber,
            PageSize = pagedJobApplications.PageSize
        };
    }
}
