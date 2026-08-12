using AcademicAppoinment.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicAppoinment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FileUploadController : ControllerBase
    {
        private readonly ISupabaseStorageService _storageService;

        public FileUploadController(ISupabaseStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] string? folder)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Vui lòng chọn file để upload." });
            }

            var fileUrl = await _storageService.UploadFileAsync(file, folder ?? "attachments");
            return Ok(new
            {
                fileName = file.FileName,
                fileUrl = fileUrl,
                fileSize = file.Length,
                contentType = file.ContentType
            });
        }
    }
}
