using CVTrack.Application.Audits.Queries;
using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Domain.Common;
using CVTrack.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVTrack.Api.Controllers;

[ApiController]
[Route("api/admin/audits")]
[Authorize(Roles = "Admin")]
public class AuditsAdminController : ControllerBase
{
    private readonly IAdminAuditService _adminAudit;

    public AuditsAdminController(IAdminAuditService adminAudit) => _adminAudit = adminAudit;

    /// GET api/admin/audits?cvId={cvId}&userId={userId}
    [HttpGet("getByUserId")]
    public async Task<ActionResult<PagedResult<AuditDto>>> GetByUserId(
            [FromQuery] Guid userId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] AuditAction? action = null)
    {
        var query = new GetAllAuditsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            Action = action,
            UserId = userId
        };

        var result = await _adminAudit.GetAllPagedAsync(query);
        AddPaginationHeaders(result);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<AuditDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] AuditAction? action = null)
    {
        var query = new GetAllAuditsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            Action = action
        };

        var result = await _adminAudit.GetAllPagedAsync(query);

        AddPaginationHeaders(result);

        return Ok(result);
    }

    private void AddPaginationHeaders<T>(PagedResult<T> pagedResult)
    {
        Response.Headers.Append("X-Pagination-TotalCount", pagedResult.TotalCount.ToString());
        Response.Headers.Append("X-Pagination-TotalPages", pagedResult.TotalPages.ToString());
        Response.Headers.Append("X-Pagination-CurrentPage", pagedResult.PageNumber.ToString());
        Response.Headers.Append("X-Pagination-HasNext", pagedResult.HasNextPage.ToString());
        Response.Headers.Append("X-Pagination-HasPrevious", pagedResult.HasPreviousPage.ToString()
        );
    }
}
