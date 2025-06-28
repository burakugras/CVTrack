using CVTrack.Application.CVs.Queries;
using CVTrack.Application.DTOs;
using CVTrack.Domain.Common;

namespace CVTrack.Application.Interfaces;

public interface IAdminCvService
{
    Task<IEnumerable<AdminCvDto>> GetAllAsync(GetAllCvsQuery query);
    Task<AdminCvDto> GetByIdAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task<PagedResult<AdminCvDto>> GetAllPagedAsync(GetAllCvsQuery query);
}
