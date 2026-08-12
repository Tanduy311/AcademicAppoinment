namespace AcademicAppoinment.Services.Interfaces
{
    public interface ISupabaseStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string? folder = null);
    }
}
