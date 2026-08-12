using AcademicAppoinment.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AcademicAppoinment.Services
{
    public class SupabaseStorageService : ISupabaseStorageService, IAvatarStorageService
    {
        private static readonly HashSet<string> AllowedAvatarExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        private static readonly HashSet<string> AllowedAvatarContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/webp"
        };

        private const long MaxAvatarSizeBytes = 2 * 1024 * 1024;
        private const long MaxUploadSizeBytes = 10 * 1024 * 1024;

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public SupabaseStorageService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string? folder = null)
        {
            ValidateGenericFile(file);

            var projectUrl = GetRequiredSetting("Supabase:ProjectUrl").TrimEnd('/');
            var bucketName = GetBucketName();

            var fileExtension = Path.GetExtension(file.FileName);
            var safeFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = CombineObjectPath(folder, safeFileName);
            var uploadUrl = $"{projectUrl}/storage/v1/object/{Uri.EscapeDataString(bucketName)}/{EncodeObjectPath(filePath)}";

            await UploadObjectAsync(file, uploadUrl, upsert: false);

            return $"{projectUrl}/storage/v1/object/public/{Uri.EscapeDataString(bucketName)}/{EncodeObjectPath(filePath)}";
        }

        public async Task<AvatarUploadResult> UploadAvatarAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            ValidateAvatarFile(file);

            var projectUrl = GetRequiredSetting("Supabase:ProjectUrl").TrimEnd('/');
            var bucketName = GetBucketName();
            var blobName = $"avatars/avatar-{Guid.NewGuid():N}{Path.GetExtension(file.FileName).ToLowerInvariant()}";
            var uploadUrl = $"{projectUrl}/storage/v1/object/{Uri.EscapeDataString(bucketName)}/{EncodeObjectPath(blobName)}";

            await UploadObjectAsync(file, uploadUrl, upsert: false, cancellationToken);

            var publicUrl = $"{projectUrl}/storage/v1/object/public/{Uri.EscapeDataString(bucketName)}/{EncodeObjectPath(blobName)}";
            return new AvatarUploadResult(publicUrl, blobName);
        }

        public async Task DeleteAvatarAsync(string? blobName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                return;
            }

            var projectUrl = GetRequiredSetting("Supabase:ProjectUrl").TrimEnd('/');
            var bucketName = GetBucketName();
            var deleteUrl = $"{projectUrl}/storage/v1/object/{Uri.EscapeDataString(bucketName)}";
            var payload = JsonSerializer.Serialize(new { prefixes = new[] { blobName } });

            using var request = CreateRequest(HttpMethod.Delete, deleteUrl);
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            using var response = await client.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        private async Task UploadObjectAsync(IFormFile file, string uploadUrl, bool upsert, CancellationToken cancellationToken = default)
        {
            using var stream = file.OpenReadStream();
            using var request = CreateRequest(HttpMethod.Post, uploadUrl);
            request.Headers.TryAddWithoutValidation("x-upsert", upsert ? "true" : "false");
            request.Content = new StreamContent(stream);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(
                string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType);

            var client = _httpClientFactory.CreateClient();
            using var response = await client.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"Lỗi khi tải file lên Supabase Storage: {response.StatusCode} - {errorMsg}");
            }
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string url)
        {
            var serviceKey = GetRequiredSetting("Supabase:ServiceRoleKey");
            var request = new HttpRequestMessage(method, url);
            request.Headers.TryAddWithoutValidation("apikey", serviceKey);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", serviceKey);
            return request;
        }

        private static void ValidateGenericFile(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                throw new ArgumentException("File upload không hợp lệ.");
            }

            if (file.Length > MaxUploadSizeBytes)
            {
                throw new ArgumentException("File upload không được vượt quá 10MB.");
            }
        }

        private static void ValidateAvatarFile(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                throw new ArgumentException("Vui lòng chọn file ảnh.");
            }

            if (file.Length > MaxAvatarSizeBytes)
            {
                throw new ArgumentException("Ảnh đại diện không được vượt quá 2MB.");
            }

            var ext = Path.GetExtension(file.FileName);
            if (!AllowedAvatarExtensions.Contains(ext))
            {
                throw new ArgumentException("Chỉ hỗ trợ file JPG, JPEG, PNG hoặc WEBP.");
            }

            if (!AllowedAvatarContentTypes.Contains(file.ContentType ?? string.Empty))
            {
                throw new ArgumentException("File upload phải là ảnh hợp lệ.");
            }
        }

        private string GetBucketName()
        {
            return _configuration["Supabase:AvatarBucket"]
                ?? _configuration["Supabase:Bucket"]
                ?? "store-academic-appointment";
        }

        private string GetRequiredSetting(string key)
        {
            var value = _configuration[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"{key} is not configured.");
            }

            return value;
        }

        private static string CombineObjectPath(string? folder, string fileName)
        {
            if (string.IsNullOrWhiteSpace(folder))
            {
                return fileName;
            }

            var safeFolder = string.Join('/',
                folder.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(segment => segment != "." && segment != ".."));

            return string.IsNullOrWhiteSpace(safeFolder) ? fileName : $"{safeFolder}/{fileName}";
        }

        private static string EncodeObjectPath(string objectPath)
        {
            return string.Join('/', objectPath.Split('/', StringSplitOptions.RemoveEmptyEntries).Select(Uri.EscapeDataString));
        }
    }
}
