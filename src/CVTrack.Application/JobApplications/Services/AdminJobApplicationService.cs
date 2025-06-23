using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Application.JobApplications.Commands;
using CVTrack.Application.JobApplications.Queries;

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
}
