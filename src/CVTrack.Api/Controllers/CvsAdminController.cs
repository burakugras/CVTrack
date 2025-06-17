using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Application.CVs.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVTrack.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class CvsAdminController : ControllerBase
{
    private readonly IAdminCvService _adminCv;

    public CvsAdminController(IAdminCvService adminCv)
        => _adminCv = adminCv;

    // GET api/admin/CvsAdmin
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdminCvDto>>> GetAll()
    {
        var list = await _adminCv.GetAllAsync(new GetAllCvsQuery());
        return Ok(list);
    }

    // DELETE api/admin/CvsAdmin/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _adminCv.DeleteAsync(id);
        return NoContent();
    }
}

