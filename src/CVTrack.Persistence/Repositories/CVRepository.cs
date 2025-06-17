using CVTrack.Application.Interfaces;
using CVTrack.Domain.Entities;
using CVTrack.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace CVTrack.Persistence.Repositories;

public class CVRepository : ICVRepository
{

    private readonly AppDbContext _context;
    public CVRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<CV?> GetByIdAsync(Guid id)
    {
        return await _context.CVs
            .Include(c => c.User)
            .Include(c => c.JobApplications)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<CV>> GetByUserIdAsync(Guid userId)
    {
        return await _context.CVs
            .Where(c => c.UserId == userId)
            .Include(c => c.JobApplications)
            .ToListAsync();
    }

    public async Task<CV> AddAsync(CV cv)
    {
        _context.CVs.Add(cv);
        await _context.SaveChangesAsync();
        return cv;
    }

    public async Task RemoveAsync(CV cv)
    {
        _context.CVs.Remove(cv);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CV>> GetAllAsync()
    {
        return await _context.CVs
                             .Include(c => c.User)
                             .ToListAsync();
    }

    public async Task UpdateAsync(CV cv)
    {
        _context.CVs.Update(cv);
        await _context.SaveChangesAsync();
    }
}