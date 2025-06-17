using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Application.CVs.Queries;

namespace CVTrack.Application.CVs.Services;

public class AdminCvService : IAdminCvService
{
    private readonly ICVRepository _repo;

    public AdminCvService(ICVRepository repo) => _repo = repo;

    public async Task<IEnumerable<AdminCvDto>> GetAllAsync(GetAllCvsQuery _)
    {
        var cvs = await _repo.GetAllAsync();
        return cvs.Select(c => new AdminCvDto
        {
            Id = c.Id,
            UserId = c.UserId,
            UserFirstName = c.User!.FirstName,  // Include ile yüklü
            UserLastName = c.User.LastName,
            UserEmail = c.User.Email,
            FileName = c.FileName,
            UploadDate = c.UploadDate
        });
    }

    public async Task DeleteAsync(Guid id)
    {
        var cv = await _repo.GetByIdAsync(id)
               ?? throw new KeyNotFoundException($"CV Id={id} bulunamadı.");

        await _repo.RemoveAsync(cv);
    }
}

