using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using CVTrack.Api.Models;               // UploadCvRequest
using CVTrack.Application.CVs.Commands;
using CVTrack.Application.CVs.Queries;
using CVTrack.Application.CVs.Services;
using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace CVTrack.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CvsController : ControllerBase
    {
        private readonly CvService _cvService;
        private readonly IFileService _fileService;
        private readonly IAuditService _auditService;

        public CvsController(CvService cvService, IFileService fileService, IAuditService auditService)
        {
            _cvService = cvService;
            _fileService = fileService;
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CVDto>>> GetByUser()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                                     ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

            var cvs = await _cvService.GetByUserAsync(new GetCvsByUserQuery { UserId = userId });
            return Ok(cvs);
        }

        // POST /api/cvs
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<CVDto>> Upload([FromForm] UploadCvRequest request)
        {
            var file = request.File;
            if (file == null || file.Length == 0)
                return BadRequest("Lütfen yüklemek için bir PDF dosyası seçin.");

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                                     ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";

            byte[] content;
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            content = ms.ToArray();

            await _fileService.SaveFileAsync(content, fileName);

            var cmd = new CreateCvCommand
            {
                UserId = userId,
                FileName = fileName
            };
            var created = await _cvService.CreateAsync(cmd);

            return CreatedAtAction(nameof(GetByUser), new { id = created.Id }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var cv = await _cvService.GetByIdAsync(id);
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                                     ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
            if (cv.UserId != userId)
                return Forbid();

            await _cvService.DeleteAsync(id);
            await _fileService.DeleteFileAsync(cv.FileName);

            return NoContent();
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(Guid id)
        {
            var cvDto = await _cvService.GetByIdAsync(id);
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                                     ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
            if (cvDto.UserId != userId)
                return Forbid();

            await _auditService.LogDownloadAsync(userId, id);

            byte[] content;
            try
            {
                content = await _fileService.GetFileAsync(cvDto.FileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound("Dosya diskte bulunamadı.");
            }

            return File(content, "application/pdf", cvDto.FileName);
        }
    }
}
