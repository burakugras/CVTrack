using CVTrack.Application.CVs.Commands;
using CVTrack.Application.CVs.Queries;
using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Domain.Entities;

namespace CVTrack.Application.CVs.Services;

public class CvService
{
    private readonly ICVRepository _cvRepository;
    private readonly IFileService _fileService;

    public CvService(ICVRepository cvRepository, IFileService fileService)
    {
        _cvRepository = cvRepository;
        _fileService = fileService;
    }

    public async Task<CVDto> CreateAsync(CreateCvCommand cmd)
    {
        var cv = new CV
        {
            Id = Guid.NewGuid(),
            UserId = cmd.UserId,
            FileName = cmd.FileName,
            UploadDate = DateTime.UtcNow
        };

        var created = await _cvRepository.AddAsync(cv);

        return new CVDto
        {
            Id = created.Id,
            UserId = created.UserId,
            FileName = created.FileName,
            UploadDate = created.UploadDate
        };
    }

    public async Task<IEnumerable<CVDto>> GetByUserAsync(GetCvsByUserQuery query)
    {
        var cvs = await _cvRepository.GetByUserIdAsync(query.UserId);
        return cvs.Select(c => new CVDto
        {
            Id = c.Id,
            UserId = c.UserId,
            FileName = c.FileName,
            UploadDate = c.UploadDate
        });
    }

    public async Task DeleteAsync(Guid id)
    {
        var cv = await _cvRepository.GetByIdAsync(id)
             ?? throw new KeyNotFoundException($"CV with Id={id} not found.");

        // await _fileService.DeleteFileAsync(cv.FileName);  // Dosyayı silmek

        cv.IsDeleted = true;

        await _cvRepository.UpdateAsync(cv);
    }

    public async Task<CVDto> GetByIdAsync(Guid id)
    {
        var c = await _cvRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"CV with Id={id} not found.");

        return new CVDto
        {
            Id = c.Id,
            UserId = c.UserId,
            FileName = c.FileName,
            UploadDate = c.UploadDate
        };
    }
}