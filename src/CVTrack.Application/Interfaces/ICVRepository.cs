using CVTrack.Domain.Entities;

namespace CVTrack.Application.Interfaces;

public interface ICVRepository
{
    Task<CV?> GetByIdAsync(Guid id);
    Task<IEnumerable<CV>> GetByUserIdAsync(Guid userId);
    Task<CV> AddAsync(CV cv);
    Task RemoveAsync(CV cv);
}