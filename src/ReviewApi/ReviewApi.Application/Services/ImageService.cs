using System.IO;
using ReviewApi.Application.Interfaces;
using ReviewApi.Domain.Interfaces.Repositories;
using ReviewApi.Shared.Interfaces;

namespace ReviewApi.Application.Services
{
    public class ImageService : IImageService
    {
        private readonly IFileUploadUtils _fileUploadUtils;
        public ImageService(IFileUploadUtils fileUploadUtils)
        {
            _fileUploadUtils = fileUploadUtils;
        }

        public Stream GetImageByName(string name)
        {
            return _fileUploadUtils.GetImage(name);
        }
    }
}
