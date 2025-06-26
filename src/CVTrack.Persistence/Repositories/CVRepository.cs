using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using CVTrack.Application.Interfaces;
using CVTrack.Domain.Entities;
using CVTrack.Domain.Common;
using CVTrack.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace CVTrack.Persistence.Repositories
{
    public class CVRepository : ICVRepository
    {
        private readonly AppDbContext _context;

        public CVRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CV?> GetByIdAsync(Guid id)
            => await _context.CVs
                .Where(c => c.Id == id && !c.IsDeleted)
                .Include(c => c.User)
                .Include(c => c.JobApplications)
                .FirstOrDefaultAsync();

        public async Task<IEnumerable<CV>> GetByUserIdAsync(Guid userId)
            => await _context.CVs
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .Include(c => c.User)
                .Include(c => c.JobApplications)
                .ToListAsync();

        public async Task<CV> AddAsync(CV cv)
        {
            _context.CVs.Add(cv);
            await _context.SaveChangesAsync();
            return cv;
        }

        public async Task UpdateAsync(CV cv)
        {
            _context.CVs.Update(cv);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(CV cv)
        {
            _context.CVs.Remove(cv);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CV>> GetAllAsync()
            => await _context.CVs
                .Where(c => !c.IsDeleted)
                .Include(c => c.User)
                .ToListAsync();

        public async Task<PagedResult<CV>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.CVs
                .Where(c => !c.IsDeleted)
                .Include(c => c.User);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.UploadDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<CV>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<CV>> SearchCVsPagedAsync(int pageNumber, int pageSize, string searchTerm)
        {
            var query = _context.CVs
                .Where(c => !c.IsDeleted &&
                            (c.FileName.Contains(searchTerm) ||
                             c.User!.FirstName.Contains(searchTerm) ||
                             c.User.LastName.Contains(searchTerm)))
                .Include(c => c.User);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.UploadDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<CV>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<CV>> GetCvsByUserPagedAsync(int pageNumber, int pageSize, Guid userId)
        {
            var query = _context.CVs
                .Where(c => !c.IsDeleted && c.UserId == userId)
                .Include(c => c.User);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.UploadDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<CV>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<CV>> SearchCvsByUserPagedAsync(int pageNumber, int pageSize, Guid userId, string searchTerm)
        {
            var query = _context.CVs
                .Where(c => !c.IsDeleted
                            && c.UserId == userId
                            && (c.FileName.Contains(searchTerm) ||
                                c.User!.FirstName.Contains(searchTerm) ||
                                c.User.LastName.Contains(searchTerm)))
                .Include(c => c.User);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.UploadDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<CV>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
