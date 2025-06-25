using CVTrack.Application.Interfaces;
using CVTrack.Domain.Common;
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

    public async Task<PagedResult<Audit>> GetAuditsByActionPagedAsync(int pageNumber, int pageSize, AuditAction action)
    {
        var query = _context.Audits.AsQueryable();

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Audit>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<Audit>> GetAuditsByUserPagedAsync(int pageNumber, int pageSize, Guid userId)
    {
        var q = _context.Audits
                    .Where(a => a.UserId == userId);

        var totalCount = await q.CountAsync();
        var items = await q
            .OrderBy(a => a.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return new PagedResult<Audit>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<Audit>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = _context.Audits.AsQueryable();

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Audit>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<Audit>> SearchAuditsPagedAsync(int pageNumber, int pageSize, string searchTerm)
    {
        var query = _context.Audits.Where(u =>
                u.FirstName.Contains(searchTerm) ||
                u.LastName.Contains(searchTerm));

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Audit>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}