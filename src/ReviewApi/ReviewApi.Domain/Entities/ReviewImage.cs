using System;

namespace ReviewApi.Domain.Entities
{
    public class ReviewImage : BaseImage
    {
        public Guid ReviewId { get; protected set; }
        public Review Review { get; protected set; }

        public ReviewImage()
        {

        }

        public ReviewImage(string filename, string filepath)
        {
            FileName = filename;
            FilePath = filepath;
        }

        public void Update(string filename, string filepath)
        {
            FileName = filename;
            FilePath = filepath;
        }
    }
}
