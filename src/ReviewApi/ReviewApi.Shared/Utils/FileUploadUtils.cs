using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ReviewApi.Domain.Dto;
using ReviewApi.Shared.Interfaces;

namespace ReviewApi.Shared.Utils
{
    public class FileUploadUtils : IFileUploadUtils
    {
        private readonly string _rootPath;

        public FileUploadUtils(string rootPath)
        {
            _rootPath = rootPath;
        }

        public Stream GetDefaultUserProfileImage()
        {
            return File.OpenRead($@"{_rootPath}\\Images\\default-user-profile-image.jpg");
        }

        public async Task<FileDTO> UploadFile(Stream stream)
        {
            string imagePath = @"\Upload\Images\";
            string uploadPath = $"{_rootPath}{imagePath}";

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            string fileName = Guid.NewGuid().ToString();
            string filenameWithExtension = Path.GetFileName($"{fileName}.jpg");
            string fullPath = $"{uploadPath}{filenameWithExtension}";
            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }

            return new FileDTO() { FileName = filenameWithExtension, FilePath = fullPath };
        }
    }
}
