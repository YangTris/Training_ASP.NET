using Microsoft.AspNetCore.Http;

namespace Application.IServices
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Upload a file to storage and return the URL
        /// </summary>
        /// <param name="file">The file to upload</param>
        /// <param name="folder">The folder/path in storage (e.g., "products")</param>
        /// <returns>The public URL of the uploaded file</returns>
        Task<string> UploadFileAsync(IFormFile file, string folder = "products");

        /// <summary>
        /// Upload multiple files to storage
        /// </summary>
        /// <param name="files">The files to upload</param>
        /// <param name="folder">The folder/path in storage</param>
        /// <returns>List of public URLs of uploaded files</returns>
        Task<List<string>> UploadFilesAsync(IEnumerable<IFormFile> files, string folder = "products");

        /// <summary>
        /// Delete a file from storage
        /// </summary>
        /// <param name="fileUrl">The URL of the file to delete</param>
        Task DeleteFileAsync(string fileUrl);

        /// <summary>
        /// Validate if file is a valid image
        /// </summary>
        /// <param name="file">The file to validate</param>
        /// <returns>True if valid image, false otherwise</returns>
        bool IsValidImage(IFormFile file);
    }
}
