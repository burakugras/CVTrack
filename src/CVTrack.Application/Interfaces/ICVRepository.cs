using CVTrack.Domain.Common;
using CVTrack.Domain.Entities;

namespace CVTrack.Application.Interfaces;

public interface ICVRepository
{
    Task<CV?> GetByIdAsync(Guid id);
    Task<IEnumerable<CV>> GetByUserIdAsync(Guid userId);
    Task<CV> AddAsync(CV cv);
    Task<IEnumerable<CV>> GetAllAsync();
    Task UpdateAsync(CV cv);
    Task RemoveAsync(CV cv);

    Task<PagedResult<CV>> GetPagedAsync(int pageNumber, int pageSize);
    Task<PagedResult<CV>> SearchCVsPagedAsync(int pageNumber, int pageSize, string searchTerm);
    Task<PagedResult<CV>> GetCvsByUserPagedAsync(int pageNumber, int pageSize, Guid userId);
    Task<PagedResult<CV>> SearchCvsByUserPagedAsync(int pageNumber, int pageSize, Guid userId, string searchTerm);
}
