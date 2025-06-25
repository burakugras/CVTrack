using CVTrack.Application.Audits.Queries;
using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Domain.Common;
using CVTrack.Domain.Entities;

namespace CVTrack.Application.Audits.Services;

public class AdminAuditService : IAdminAuditService
{
    private readonly IAuditRepository _auditRepo;

    public AdminAuditService(IAuditRepository auditRepo)
        => _auditRepo = auditRepo;

    public async Task<IEnumerable<AuditDto>> GetAllAsync(GetAuditsQuery query)
    {
        var entries = await _auditRepo.GetAllAsync();
        var list = entries
            // .Where(a => !query.CvId.HasValue || a.CvId == query.CvId)
            .Where(a => !query.UserId.HasValue || a.UserId == query.UserId)
            .Select(a => new AuditDto
            {
                Id = a.Id,
                UserId = a.UserId,
                CvId = a.CvId,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Action = a.Action.ToString(),
                Timestamp = a.Timestamp
            });
        return list;
    }

    public async Task<PagedResult<AuditDto>> GetAllPagedAsync(GetAllAuditsQuery query)
    {
        var pagination = new PaginationRequest
        {
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };

        PagedResult<Audit> pagedAudits;

        if (query.UserId.HasValue)
        {
            pagedAudits = await _auditRepo.GetAuditsByUserPagedAsync(
                pagination.ValidatedPageNumber,
                pagination.ValidatedPageSize,
                query.UserId.Value
            );
        }
        // Search varsa
        else if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            pagedAudits = await _auditRepo.SearchAuditsPagedAsync(
                pagination.ValidatedPageNumber,
                pagination.ValidatedPageSize,
                query.SearchTerm
            );
        }
        // Role filter varsa
        else if (query.Action.HasValue)
        {
            pagedAudits = await _auditRepo.GetAuditsByActionPagedAsync(
                pagination.ValidatedPageNumber,
                pagination.ValidatedPageSize,
                query.Action.Value
            );
        }

        // Normal pagination
        else
        {
            pagedAudits = await _auditRepo.GetPagedAsync(
                pagination.ValidatedPageNumber,
                pagination.ValidatedPageSize
            );
        }
        // User'ları AdminUserDto'ya dönüştür
        var adminAuditDtos = pagedAudits.Items.Select(u => new AuditDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            UserId = u.UserId,
            CvId = u.CvId,
            Timestamp = u.Timestamp,
            Action = u.Action.ToString()
        });

        return new PagedResult<AuditDto>
        {
            Items = adminAuditDtos,
            TotalCount = pagedAudits.TotalCount,
            PageNumber = pagedAudits.PageNumber,
            PageSize = pagedAudits.PageSize
        };
    }

    // public async Task<IEnumerable<AuditDto>> GetAllAuditsAsync()
    // {
    //     var entries = await _auditRepo.GetAllAsync();
    //     var list = entries
    //         .Select(a => new AuditDto
    //         {
    //             Id = a.Id,
    //             UserId = a.UserId,
    //             CvId = a.CvId,
    //             FirstName = a.FirstName,
    //             LastName = a.LastName,
    //             Action = a.Action.ToString(),
    //             Timestamp = a.Timestamp
    //         });

    //     return list;
    // }
}

