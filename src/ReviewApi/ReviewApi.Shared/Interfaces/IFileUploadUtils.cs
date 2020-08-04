using System.IO;
using System.Threading.Tasks;
using ReviewApi.Domain.Dto;

namespace ReviewApi.Shared.Interfaces
{
    public interface IFileUploadUtils
    {
        Task<FileDTO> UploadFile(Stream fileStream);
        Stream GetDefaultUserProfileImage();
        Stream GetImage(string name);
        string GenerateImageUrl(string imageName);
    }
}
