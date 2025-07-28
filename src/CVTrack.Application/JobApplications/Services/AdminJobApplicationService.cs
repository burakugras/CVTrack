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

    public AdminJobApplicationService(IJobApplicationRepository repo) => _repo = repo;

    // tüm verileri getir (soft delete durumu fark etmeksizin)
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
            FileName = j.FileName,
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

    // sadece aktif verileri getir (IsDeleted = false) - sayfalanmış
    public async Task<PagedResult<AdminJobApplicationDto>> GetAllActivePagedJobApplications(GetAllJobApplicationsQuery query)
    {
        var pagination = new PaginationRequest
        {
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };

        // Bu metod sadece aktif (soft delete edilmemiş) verileri getirecek
        var pagedJobApplications = await _repo.GetAllActiveJobApplicationsAsync(
            pagination.ValidatedPageNumber,
            pagination.ValidatedPageSize,
            query.SearchTerm,
            query.Status
        );

        var adminActiveJobApplications = pagedJobApplications.Items.Select(u => new AdminJobApplicationDto
        {
            Id = u.Id,
            CVId = u.CVId,
            UserId = u.UserId,
            UserFirstName = u.User?.FirstName ?? string.Empty,
            UserLastName = u.User?.LastName ?? string.Empty,
            UserMail = u.User?.Email ?? string.Empty,
            FileName = u.FileName,
            CompanyName = u.CompanyName,
            ApplicationDate = u.ApplicationDate,
            Status = u.Status,
            Notes = u.Notes
        });

        return new PagedResult<AdminJobApplicationDto>
        {
            Items = adminActiveJobApplications,
            TotalCount = pagedJobApplications.TotalCount,
            PageNumber = pagedJobApplications.PageNumber,
            PageSize = pagedJobApplications.PageSize
        };
    }

    // tüm verileri getir (soft delete durumu fark etmeksizin) - sayfalanmış
    public async Task<PagedResult<AdminJobApplicationDto>> GetAllPagedAsync(GetAllJobApplicationsQuery query)
    {
        var pagination = new PaginationRequest
        {
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };

        // Bu metod tüm verileri (soft delete edilmiş olanlar dahil) getirecek
        var pagedJobApplications = await _repo.GetAllJobApplicationsPagedAsync(
            pagination.ValidatedPageNumber,
            pagination.ValidatedPageSize,
            query.SearchTerm,
            query.Status
        );

        // JobApplications'ları AdminJobApplicationDto'ya dönüştür
        var adminJobApplicationDtos = pagedJobApplications.Items.Select(u => new AdminJobApplicationDto
        {
            Id = u.Id,
            UserId = u.UserId,
            UserFirstName = u.User?.FirstName ?? string.Empty,
            UserLastName = u.User?.LastName ?? string.Empty,
            UserMail = u.User?.Email ?? string.Empty,
            FileName = u.FileName,
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