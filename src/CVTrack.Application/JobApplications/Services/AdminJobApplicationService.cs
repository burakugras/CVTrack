
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
            UserFirstName = j.User.FirstName,
            UserLastName = j.User.LastName,
            UserMail = j.User.Email,
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

        await _repo.RemoveAsync(j);
    }
}

