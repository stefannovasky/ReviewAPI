using System;

namespace ReviewApi.Domain.Entities
{
    public class BaseImage : BaseEntity
    {

        public string FileName { get; protected set; }
        public string FilePath { get; protected set; }

        public BaseImage() : base()
        {

        }

        public BaseImage(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
        }

        public BaseImage(Guid id) : base(id)
        {

        }

        public BaseImage(Guid id, string fileName, string filePath) : base(id)
        {
            FileName = fileName;
            FilePath = filePath;
        }

        public void Update(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
        }
    }
}
