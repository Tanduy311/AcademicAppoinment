using Microsoft.AspNetCore.Http;

namespace AcademicAppoinment.DTOs
{
    public class UploadProgressRequest
    {
        public IFormFile File { get; set; }
        public string? Description { get; set; }
    }
}
