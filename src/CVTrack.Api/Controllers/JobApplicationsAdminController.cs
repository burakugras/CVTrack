using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Application.JobApplications.Commands;
using CVTrack.Application.JobApplications.Queries;
using CVTrack.Domain.Common;
using CVTrack.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVTrack.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class JobApplicationsAdminController : ControllerBase
{
    private readonly IAdminJobApplicationService _admin;

    public JobApplicationsAdminController(IAdminJobApplicationService admin) => _admin = admin;

    // GET api/admin/JobApplicationsAdmin
    /*
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdminJobApplicationDto>>> GetAll()
    {
        var list = await _admin.GetAllAsync(new GetAllJobApplicationsQuery());
        return Ok(list);
    }
    */

    [HttpGet("getAll")]
    public async Task<ActionResult<PagedResult<AdminJobApplicationDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] ApplicationStatus? status = null
    )
    {
        var query = new GetAllJobApplicationsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            Status = status,
        };

        var result = await _admin.GetAllPagedAsync(query);

        // Response header'larına pagination bilgilerini ekle
        AddPaginationHeaders(result);

        return Ok(result);
    }

    [HttpGet("getAllActive")]
    public async Task<ActionResult<PagedResult<AdminJobApplicationDto>>> GetActiveJobApplicationsForAdmin(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] ApplicationStatus? status = null
    )
    {
        var query = new GetAllJobApplicationsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            Status = status
        };

        var result = await _admin.GetAllActivePagedJobApplications(query);

        AddPaginationHeaders(result);

        return Ok(result);
    }

    // PUT api/admin/JobApplicationsAdmin/{id}/status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateJobApplicationStatusCommand cmd
    )
    {
        if (id != cmd.Id)
            return BadRequest("URL ve body Id uyuşmuyor.");
        await _admin.UpdateStatusAsync(cmd);
        return NoContent();
    }

    // DELETE api/admin/JobApplicationsAdmin/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _admin.DeleteAsync(new DeleteJobApplicationCommand { Id = id });
        return NoContent();
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
