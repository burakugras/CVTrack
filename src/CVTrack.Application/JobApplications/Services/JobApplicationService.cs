using CVTrack.Application.Interfaces;
using CVTrack.Application.JobApplications.Commands;
using CVTrack.Domain.Entities;

namespace CVTrack.Application.JobApplications.Services;

public class JobApplicationService
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
        await _jobRepo.RemoveAsync(existing);
    }

    public async Task<IEnumerable<JobApplication>> GetByUserAsync(Guid userId)
        => await _jobRepo.GetByUserIdAsync(userId);

    public async Task<JobApplication?> GetByIdAsync(Guid id)
        => await _jobRepo.GetByIdAsync(id);
}