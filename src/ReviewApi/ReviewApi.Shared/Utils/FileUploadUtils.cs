using System;
using System.IO;
using System.Threading.Tasks;
using ReviewApi.Domain.Dto;
using ReviewApi.Domain.Exceptions;
using ReviewApi.Shared.Interfaces;

namespace ReviewApi.Shared.Utils
{
    public class FileUploadUtils : IFileUploadUtils
    {
        private readonly string _rootPath;
        private readonly string _webApplicationUrl;

        public FileUploadUtils(string rootPath, string webApplicationUrl)
        {
            _rootPath = rootPath;
            _webApplicationUrl = webApplicationUrl;
        }

        public Stream GetImage(string name)
        {
            try
            {
                return File.OpenRead($"{_rootPath}\\Upload\\Images\\{name}");
            }
            catch (FileNotFoundException)
            {
                throw new ResourceNotFoundException("image not found.");
            }
        }

        public Stream GetDefaultUserProfileImage()
        {
            return File.OpenRead($@"{_rootPath}\\Images\\default-user-profile-image.jpg");
        }

        public async Task<FileDTO> UploadImage(Stream imageStream)
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
                await imageStream.CopyToAsync(fileStream);
            }

            return new FileDTO() { FileName = filenameWithExtension, FilePath = fullPath };
        }

        public string GenerateImageUrl(string imageName)
        {
            return $"{_webApplicationUrl}/images/{imageName}";
        }
    }
}
