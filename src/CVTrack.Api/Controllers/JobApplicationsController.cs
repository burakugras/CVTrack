using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CVTrack.Application.DTOs;
using CVTrack.Application.JobApplications.Commands;
using CVTrack.Application.JobApplications.Queries;
using CVTrack.Application.JobApplications.Services;
using CVTrack.Domain.Common;
using CVTrack.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVTrack.Api.Controllers
{
    [ApiController]
    [Route("api/job-applications")]
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
            Guid.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)!
            );

        // GET: api/jobapplications
        [HttpGet]
        public async Task<ActionResult<PagedResult<JobApplicationDto>>> GetByUser(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] ApplicationStatus? status = null)
        {
            var userId = GetUserId();
            var jobApplication = new GetAllJobApplicationsQuery
            {
                UserId = userId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                Status = status

            };

            var result = await _jobService.GetAllPagedAsync(jobApplication);
            AddPaginationHeaders(result);

            return Ok(result);
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
                Id = entity.Id,
                UserId = entity.UserId,
                CVId = entity.CVId,
                FileName = entity.FileName,
                CompanyName = entity.CompanyName,
                ApplicationDate = entity.ApplicationDate,
                Status = entity.Status,
                Notes = entity.Notes,
            };
            return Ok(dto);
        }

        // POST: api/jobapplications
        [HttpPost]
        public async Task<ActionResult<JobApplicationDto>> Create(
            [FromBody] CreateJobApplicationCommand cmd
        )
        {
            cmd.UserId = GetUserId();
            var created = await _jobService.CreateAsync(cmd);

            var dto = new JobApplicationDto
            {
                Id = created.Id,
                UserId = created.UserId,
                CVId = created.CVId,
                FileName = created.FileName,
                CompanyName = created.CompanyName,
                ApplicationDate = created.ApplicationDate,
                Status = created.Status,
                Notes = created.Notes,
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

        private void AddPaginationHeaders<T>(PagedResult<T> paged)
        {
            Response.Headers.Append("X-Pagination-TotalCount", paged.TotalCount.ToString());
            Response.Headers.Append("X-Pagination-TotalPages", ((paged.TotalCount + paged.PageSize - 1) / paged.PageSize).ToString());
            Response.Headers.Append("X-Pagination-CurrentPage", paged.PageNumber.ToString());
            Response.Headers.Append("X-Pagination-HasNext", paged.HasNextPage.ToString());
            Response.Headers.Append("X-Pagination-HasPrevious", paged.HasPreviousPage.ToString());
        }
    }
}
