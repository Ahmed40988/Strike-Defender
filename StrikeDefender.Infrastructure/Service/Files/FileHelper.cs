using Microsoft.AspNetCore.Http;
using System.IO;

namespace StrikeDefender.Infrastructure.Services.Files
{
    public static class FileHelper
    {
        public static string UploadFile(IFormFile file, string folderName)
        {
            string folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                folderName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = $"{Guid.NewGuid()}_{file.FileName}";
            string filePath = Path.Combine(folderPath, fileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(fileStream);

            return fileName;
        }

        public static void DeleteFile(string fileName, string folderName)
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
