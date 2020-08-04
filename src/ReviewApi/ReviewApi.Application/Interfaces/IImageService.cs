using System.IO;

namespace ReviewApi.Application.Interfaces
{
    public interface IImageService
    {
        Stream GetImageByName(string name);
    }
}
