using System;

namespace ReviewApi.Domain.Entities
{
    public class ProfileImage : BaseImage
    {
        public Guid UserId { get; protected set; }
        public User User { get; protected set; }

        public ProfileImage()
        {

        }

        public ProfileImage(Guid id) : base(id)
        {

        }

        public ProfileImage(Guid id, string filename, string filepath) : base(id, filename, filepath)
        {

        }

        public ProfileImage(Guid id, string filename, string filepath, Guid userId) : base(id, filename, filepath)
        {
            UserId = userId;
        }

        public ProfileImage(string filename, string filepath, Guid userId) : base(filename, filepath)
        {
            UserId = userId; 
        }

        public ProfileImage(string filename, string filepath) : base(filename, filepath)
        {

        }

        public void UpdateUserId(Guid userId)
        {
            UserId = userId; 
        }
    }
}
