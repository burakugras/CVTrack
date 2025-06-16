using System.Security.Claims;
using CVTrack.Application.JobApplications.Commands;
using CVTrack.Application.JobApplications.Services;
using CVTrack.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace CVTrack.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JobApplicationsController : ControllerBase
    {
        private readonly JobApplicationService _jobService;

        public JobApplicationsController(JobApplicationService jobService)
        {
            _jobService = jobService;
        }

        // Helper: Token’dan userId al
        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                       ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

        // GET: api/jobapplications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobApplicationDto>>> GetByUser()
        {
            var userId = GetUserId();
            var entities = await _jobService.GetByUserAsync(userId);
            var dtos = entities.Select(j => new JobApplicationDto
            {
                Id              = j.Id,
                UserId          = j.UserId,
                CVId            = j.CVId,
                CompanyName     = j.CompanyName,
                ApplicationDate = j.ApplicationDate,
                Status          = j.Status,
                Notes           = j.Notes
            });
            return Ok(dtos);
        }

        // GET: api/jobapplications/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<JobApplicationDto>> GetById(Guid id)
        {
            var entity = await _jobService.GetByIdAsync(id);
            if (entity == null || entity.UserId != GetUserId())
                return NotFound();

            var dto = new JobApplicationDto
            {
                Id              = entity.Id,
                UserId          = entity.UserId,
                CVId            = entity.CVId,
                CompanyName     = entity.CompanyName,
                ApplicationDate = entity.ApplicationDate,
                Status          = entity.Status,
                Notes           = entity.Notes
            };
            return Ok(dto);
        }

        // POST: api/jobapplications
        [HttpPost]
        public async Task<ActionResult<JobApplicationDto>> Create([FromBody] CreateJobApplicationCommand cmd)
        {
            cmd.UserId = GetUserId();
            var created = await _jobService.CreateAsync(cmd);

            var dto = new JobApplicationDto
            {
                Id              = created.Id,
                UserId          = created.UserId,
                CVId            = created.CVId,
                CompanyName     = created.CompanyName,
                ApplicationDate = created.ApplicationDate,
                Status          = created.Status,
                Notes           = created.Notes
            };

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        // PUT: api/jobapplications/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateJobApplicationCommand cmd)
        {
            if (id != cmd.Id)
                return BadRequest("URL ile body içindeki Id uyuşmuyor.");

            cmd.UserId = GetUserId();
            await _jobService.UpdateAsync(cmd);
            return NoContent();
        }

        // DELETE: api/jobapplications/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _jobService.DeleteAsync(id);
            return NoContent();
        }
    }
}
