using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StrikeDefender.Application.Common.Interfaces;
using System.IO;

namespace StrikeDefender.Infrastructure.Services.Files
{

    public class FileHelper : IFileHelperService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FileHelper> _logger;

        public FileHelper(IWebHostEnvironment env,ILogger<FileHelper> logger)
        {
            _env = env;
            _logger = logger;
        }

        public string UploadFile(IFormFile file, string folderName)
        {
            string folderPath = Path.Combine(_env.WebRootPath, folderName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = $"{Guid.NewGuid()}_{file.FileName}";
            string filePath = Path.Combine(folderPath, fileName);
            _logger.LogInformation($"########Folder Path aho ====>>{filePath}####################");
            Console.WriteLine($" ########Folder Path aho ====>>{filePath}####################");
            using var stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);

            return fileName;
        }

        public void DeleteFile(string fileName, string folderName)
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                folderName,
                fileName);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }
    }
}
