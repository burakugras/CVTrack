using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Application.CVs.Queries;
using CVTrack.Domain.Common;
using CVTrack.Domain.Entities;

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

    public async Task<AdminCvDto> GetByIdAsync(Guid id)
    {
        var c = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"CV Id={id} bulunamadı.");

        return new AdminCvDto
        {
            Id = c.Id,
            UserId = c.UserId,
            UserFirstName = c.User!.FirstName,
            UserLastName = c.User.LastName,
            UserEmail = c.User.Email,
            FileName = c.FileName,
            UploadDate = c.UploadDate
        };

    }

    public async Task DeleteAsync(Guid id)
    {
        var cv = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"CV Id={id} bulunamadı.");

        cv.IsDeleted = true;

        await _repo.UpdateAsync(cv);
    }

    public async Task<PagedResult<AdminCvDto>> GetAllPagedAsync(GetAllCvsQuery query)
    {
        var pagination = new PaginationRequest
        {
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };

        PagedResult<CV> pagedCVs;

        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            pagedCVs = await _repo.SearchCVsPagedAsync(
                pagination.ValidatedPageNumber,
                pagination.ValidatedPageSize,
                query.SearchTerm
            );
        }
        else
        {
            pagedCVs = await _repo.GetPagedAsync(
                pagination.ValidatedPageNumber,
                pagination.ValidatedPageSize
            );
        }

        var adminCVDtos = pagedCVs.Items.Select(c => new AdminCvDto
        {
            Id = c.Id,
            UserId = c.UserId,
            UserFirstName = c.User.FirstName,
            UserLastName = c.User.LastName,
            UserEmail = c.User.Email,
            FileName = c.FileName,
            UploadDate = c.UploadDate
        });

        return new PagedResult<AdminCvDto>
        {
            Items = adminCVDtos,
            TotalCount = pagedCVs.TotalCount,
            PageNumber = pagedCVs.PageNumber,
            PageSize = pagedCVs.PageSize
        };

    }
}

