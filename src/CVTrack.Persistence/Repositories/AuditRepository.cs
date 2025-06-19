using CVTrack.Application.Interfaces;
using CVTrack.Domain.Entities;
using CVTrack.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace CVTrack.Persistence.Repositories;

public class AuditRepository : IAuditRepository
{
    private readonly AppDbContext _context;
    public AuditRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Audit entry)
    {
        _context.Audits.Add(entry);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Audit>> GetAllAsync()
    {
        return await _context.Audits
                                .AsNoTracking()
                                .ToListAsync();
    }
}