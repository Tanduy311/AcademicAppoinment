namespace AcademicAppoinment.Services.Interfaces
{
    public interface IAvatarStorageService
    {
        Task<AvatarUploadResult> UploadAvatarAsync(IFormFile file, CancellationToken cancellationToken = default);
        Task DeleteAvatarAsync(string? blobName, CancellationToken cancellationToken = default);
    }

    public sealed record AvatarUploadResult(string AvatarUrl, string AvatarBlobName);
}
