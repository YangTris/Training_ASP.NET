using Application.IServices;
using Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace Application.Services
{
    public class SupabaseStorageService : IFileStorageService
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;
        private readonly string _bucketName;
        private readonly string _publicUrl;
        private readonly IConfiguration _configuration;

        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public SupabaseStorageService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            // Read from configuration
            _supabaseUrl = configuration["Supabase:Url"] ?? throw new InvalidOperationException("Supabase URL not configured");
            _supabaseKey = configuration["Supabase:ServiceKey"] ?? throw new InvalidOperationException("Supabase Key not configured");
            _bucketName = configuration["Supabase:BucketName"] ?? "Training_img";
            _publicUrl = $"{_supabaseUrl}/storage/v1/object/public/{_bucketName}";

            // Set authorization header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _supabaseKey);
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
        }

        public bool IsValidImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.Length > MaxFileSize)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var isValidExtension = false;
            foreach (var allowedExt in AllowedExtensions)
            {
                if (allowedExt == extension)
                {
                    isValidExtension = true;
                    break;
                }
            }
            if (!isValidExtension)
                return false;

            return true;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder = "products")
        {
            if (!IsValidImage(file))
            {
                throw new BadRequestException(
                    $"Invalid image file. Allowed formats: {string.Join(", ", AllowedExtensions)}. Max size: {MaxFileSize / 1024 / 1024}MB"
                );
            }

            try
            {
                // Generate unique filename
                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = $"{folder}/{fileName}";

                // Prepare multipart form data
                using var content = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream();
                using var streamContent = new StreamContent(fileStream);
                
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(streamContent, "file", fileName);

                // Upload to Supabase
                var uploadUrl = $"{_supabaseUrl}/storage/v1/object/{_bucketName}/{filePath}";
                var response = await _httpClient.PostAsync(uploadUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to upload file to Supabase: {error}");
                }

                // Return public URL
                return $"{_publicUrl}/{filePath}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading file to Supabase: {ex.Message}", ex);
            }
        }

        public async Task<List<string>> UploadFilesAsync(IEnumerable<IFormFile> files, string folder = "products")
        {
            var uploadTasks = files.Select(file => UploadFileAsync(file, folder));
            var urls = await Task.WhenAll(uploadTasks);
            return urls.ToList();
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            try
            {
                // Extract file path from URL
                var uri = new Uri(fileUrl);
                var pathSegments = uri.Segments;
                
                // Find the path after /public/bucket-name/
                var publicIndex = Array.FindIndex(pathSegments, s => s.Contains("public"));
                if (publicIndex == -1 || publicIndex + 2 >= pathSegments.Length)
                {
                    throw new BadRequestException("Invalid file URL format");
                }

                var filePath = string.Join("", pathSegments.Skip(publicIndex + 2));

                // Delete from Supabase
                var deleteUrl = $"{_supabaseUrl}/storage/v1/object/{_bucketName}/{filePath}";
                var response = await _httpClient.DeleteAsync(deleteUrl);

                if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to delete file from Supabase: {error}");
                }
            }
            catch (Exception ex)
            {
                // Log error but don't throw - file deletion is not critical
                Console.WriteLine($"Error deleting file from Supabase: {ex.Message}");
            }
        }
    }
}
