using CVTrack.Application.Interfaces;
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
    public async Task<JobApplication> GetByIdAsync(Guid id)
    {
        var jobApplication = await _context.JobApplications
            .Where(j => j.Id == id && !j.IsDeleted)
            .Include(j => j.User)
            .Include(j => j.CV)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (jobApplication == null)
        {
            throw new InvalidOperationException($"JobApplication with ID {id} not found.");
        }

        return jobApplication;
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

    public async Task<IEnumerable<JobApplication>> GetAllAsync()
    {
        return await _context.JobApplications
                             .Include(j => j.CV)
                             .Include(j => j.User)
                             .ToListAsync();
    }

    public async Task<IEnumerable<JobApplication>> GetAllActiveJobApplicationsAsync()
    {
        return await _context.JobApplications
                             .Where(j => !j.IsDeleted)
                             .ToListAsync();
    }
}