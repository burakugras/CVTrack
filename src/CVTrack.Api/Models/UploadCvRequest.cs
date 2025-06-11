using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CVTrack.Api.Models
{
    public class UploadCvRequest
    {
        [FromForm(Name = "file")]
        public IFormFile File { get; set; } = null!;
    }
}
