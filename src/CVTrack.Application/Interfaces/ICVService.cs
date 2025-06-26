using CVTrack.Application.CVs.Commands;
using CVTrack.Application.CVs.Queries;
using CVTrack.Application.DTOs;
using CVTrack.Domain.Common;

namespace CVTrack.Application.Interfaces;

public interface ICVService
{
    Task<IEnumerable<CVDto>> GetByUserAsync(GetCvsByUserQuery query);
    Task<CVDto> CreateAsync(CreateCvCommand createCvCommand);
    Task DeleteAsync(Guid id);
    Task<CVDto> GetByIdAsync(Guid id);
    Task<PagedResult<CVDto>> GetAllPagedAsync(GetAllCvsQuery query);
}