using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Application.Users.Commands;
using CVTrack.Application.Users.Queries;
using CVTrack.Domain.Common;
using CVTrack.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVTrack.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin")]
    public class UsersAdminController : ControllerBase
    {
        private readonly IAdminUserService _admin;

        public UsersAdminController(IAdminUserService admin) => _admin = admin;

        // GET api/admin/users
        /*
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminUserDto>>> GetAll()
        {
            var list = await _admin.GetAllAsync(new GetAllUsersQuery());
            return Ok(list);
        }
        */

        // PUT api/admin/users/{id}/role
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateUserRoleCommand cmd)
        {
            //double entry
            if (id != cmd.UserId)
                return BadRequest("URL ve body Id uyuşmuyor.");
            await _admin.UpdateRoleAsync(cmd);
            return NoContent();
        }

        // DELETE api/admin/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _admin.DeleteAsync(new DeleteUserCommand { UserId = id });
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<AdminUserDto>>> GetAllUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] UserRole? role = null
        )
        {
            var query = new GetAllUsersQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                Role = role,
            };

            var result = await _admin.GetAllPagedAsync(query);

            // Response header'larına pagination bilgilerini ekle
            AddPaginationHeaders(result);

            return Ok(result);
        }

        // [HttpGet("all")]
        // public async Task<ActionResult<IEnumerable<AdminUserDto>>> GetAllUsersLegacy()
        // {
        //     var query = new GetAllUsersQuery();
        //     var result = await _admin.GetAllAsync(query);
        //     return Ok(result);
        // }

        private void AddPaginationHeaders<T>(PagedResult<T> pagedResult)
        {
            Response.Headers.Append("X-Pagination-TotalCount", pagedResult.TotalCount.ToString());
            Response.Headers.Append("X-Pagination-TotalPages", pagedResult.TotalPages.ToString());
            Response.Headers.Append("X-Pagination-CurrentPage", pagedResult.PageNumber.ToString());
            Response.Headers.Append("X-Pagination-HasNext", pagedResult.HasNextPage.ToString());
            Response.Headers.Append("X-Pagination-HasPrevious", pagedResult.HasPreviousPage.ToString());
        }
    }
}
