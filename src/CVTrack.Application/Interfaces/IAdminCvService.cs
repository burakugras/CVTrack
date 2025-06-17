using CVTrack.Application.CVs.Queries;
using CVTrack.Application.DTOs;

namespace CVTrack.Application.Interfaces;

public interface IAdminCvService
{
    Task<IEnumerable<AdminCvDto>> GetAllAsync(GetAllCvsQuery query);
    Task DeleteAsync(Guid id);
}