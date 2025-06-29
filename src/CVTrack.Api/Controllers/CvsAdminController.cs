using System.Security.Claims;
using CVTrack.Application.CVs.Queries;
using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CVTrack.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class CvsAdminController : ControllerBase
{
    private readonly IAdminCvService _adminCv;
    private readonly IFileService _fileService;
    private readonly IAuditService _auditService;

    public CvsAdminController(
        IAdminCvService adminCv,
        IFileService fileService,
        IAuditService auditService
    )
    {
        _adminCv = adminCv;
        _fileService = fileService;
        _auditService = auditService;
    }

    // GET api/admin/CvsAdmin
    [HttpGet("getAll")]
    public async Task<ActionResult<PagedResult<AdminCvDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null
    )
    {
        var query = new GetAllCvsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm
        };

        var result = await _adminCv.GetAllPagedAsync(query);

        AddPaginationHeaders(result);

        return Ok(result);

    }

    // DELETE api/admin/CvsAdmin/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _adminCv.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        var cvDto = await _adminCv.GetByIdAsync(id);
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _auditService.LogDownloadAsync(userId, id);

        byte[] content;

        try
        {
            content = await _fileService.GetFileAsync(cvDto.FileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound("Dosya diskte bulunmadı.");
        }

        return File(content, "application/pdf", cvDto.FileName);
    }

    private void AddPaginationHeaders<T>(PagedResult<T> pagedResult)
    {
        Response.Headers.Append("X-Pagination-TotalCount", pagedResult.TotalCount.ToString());
        Response.Headers.Append("X-Pagination-TotalPages", pagedResult.TotalPages.ToString());
        Response.Headers.Append("X-Pagination-CurrentPage", pagedResult.PageNumber.ToString());
        Response.Headers.Append("X-Pagination-HasNext", pagedResult.HasNextPage.ToString());
        Response.Headers.Append("X-Pagination-HasPrevious", pagedResult.HasPreviousPage.ToString());
    }
}
