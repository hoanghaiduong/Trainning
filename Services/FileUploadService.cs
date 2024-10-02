
using Trainning.Interfaces;

namespace Trainning.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;

        public FileUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public async Task<string> UploadSingleFile(string[] destination, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var uploadPath = Path.Combine(_environment.WebRootPath, Path.Combine(destination));

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            // Normalize the URL path by replacing backslashes with forward slashes
            var relativePath = Path.Combine(destination).Replace("\\", "/");
            return $"{relativePath}/{fileName}";
        }
        public async Task<List<string>> UploadMultipleFiles(string[] destination, List<IFormFile> files)
        {
            var filePaths = new List<string>();

            foreach (var file in files)
            {
                var filePath = await UploadSingleFile(destination, file);
                if (filePath != null)
                {
                    filePaths.Add(filePath);
                }
            }

            return filePaths;
        }

        public bool DeleteSingleFile(string filePath)
        {
            var fullPath = Path.Combine(_environment.WebRootPath, filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
        }

        public bool DeleteMultipleFiles(List<string> filePaths)
        {
            bool allDeleted = true;
            foreach (var filePath in filePaths)
            {
                var success = DeleteSingleFile(filePath);
                if (!success)
                {
                    allDeleted = false;
                }
            }
            return allDeleted;
        }
    }
}