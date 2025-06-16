using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Application.Users.Commands;
using CVTrack.Application.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVTrack.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersAdminController : ControllerBase
    {
        private readonly IAdminUserService _admin;

        public UsersAdminController(IAdminUserService admin)
            => _admin = admin;

        // GET api/admin/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminUserDto>>> GetAll()
        {
            var list = await _admin.GetAllAsync(new GetAllUsersQuery());
            return Ok(list);
        }

        // PUT api/admin/users/{id}/role
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateUserRoleCommand cmd)
        {
            if (id != cmd.UserId) return BadRequest("URL ve body Id uyuşmuyor.");
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
    }
}
