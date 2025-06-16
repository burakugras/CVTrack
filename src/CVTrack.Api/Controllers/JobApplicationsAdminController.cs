using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Application.JobApplications.Commands;
using CVTrack.Application.JobApplications.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVTrack.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class JobApplicationsAdminController : ControllerBase
{
    private readonly IAdminJobApplicationService _admin;

    public JobApplicationsAdminController(IAdminJobApplicationService admin)
        => _admin = admin;

    // GET api/admin/JobApplicationsAdmin
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdminJobApplicationDto>>> GetAll()
    {
        var list = await _admin.GetAllAsync(new GetAllJobApplicationsQuery());
        return Ok(list);
    }

    // PUT api/admin/JobApplicationsAdmin/{id}/status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateJobApplicationStatusCommand cmd)
    {
        if (id != cmd.Id) return BadRequest("URL ve body Id uyuşmuyor.");
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
}

