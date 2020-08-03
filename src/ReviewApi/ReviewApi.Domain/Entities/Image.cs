using System;

namespace ReviewApi.Domain.Entities
{
    public class Image : BaseEntity
    {

        public string FileName { get; protected set; }
        public string FilePath { get; protected set; }
        public Guid UserId { get; protected set; }
        public User User { get; protected set; }

        public Image() : base()
        {

        }

        public Image(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
        }

        public Image(Guid id) : base(id)
        {

        }

        public Image(Guid id, string fileName, string filePath) : base(id)
        {
            FileName = fileName;
            FilePath = filePath;
        }

        public Image(string fileName, string filePath, Guid userId)
        {
            FileName = fileName;
            FilePath = filePath;
            UserId = userId;
        }

        public Image(Guid id, string fileName, string filePath, Guid userId) : base(id)
        {
            FileName = fileName;
            FilePath = filePath;
            UserId = userId;
        }

        public void Update(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
        }
    }
}
