using CVTrack.Application.Audits.Queries;
using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVTrack.Api.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class AuditsAdminController : ControllerBase
{
    private readonly IAdminAuditService _adminAudit;

    public AuditsAdminController(IAdminAuditService adminAudit)
        => _adminAudit = adminAudit;


    /// GET api/admin/audits?cvId={cvId}&userId={userId}
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditDto>>> Get([FromQuery] GetAuditsQuery query)
    {
        var list = await _adminAudit.GetAllAsync(query);
        return Ok(list);
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<AuditDto>>> GetAll()
    {
        var list = await _adminAudit.GetAllAsync(new GetAuditsQuery());
        return Ok(list);
    }
}