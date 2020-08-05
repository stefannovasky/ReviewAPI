using System;
using ReviewApi.Domain.Entities;
using Xunit;

namespace ReviewApi.UnitTests.Domain.Entities
{
    public class ProfileImageTest
    {
        [Fact]
        public void ShouldConstructWithParameterlessConstructor()
        {
            ProfileImage profileImage = new ProfileImage();

            Assert.Null(profileImage.User);
            Assert.Null(profileImage.FileName);
            Assert.Null(profileImage.FilePath);
            Assert.Equal(Guid.Empty, profileImage.Id);
            Assert.Equal(Guid.Empty, profileImage.UserId);
        }

        [Fact]
        public void ShouldConstructWithIdConstructor()
        {
            Guid id = Guid.NewGuid();
            ProfileImage profileImage = new ProfileImage(id);

            Assert.Null(profileImage.User);
            Assert.Null(profileImage.FileName);
            Assert.Null(profileImage.FilePath);
            Assert.Equal(id, profileImage.Id);
            Assert.Equal(Guid.Empty, profileImage.UserId);
        }

        [Fact]
        public void ShouldConstructWithFilenameAndFilepathConstructor()
        {
            string filename = "FILENAME";
            string filepath = "FILEPATH"; 
            ProfileImage profileImage = new ProfileImage(filename, filepath);

            Assert.Null(profileImage.User);
            Assert.Equal(filename, profileImage.FileName);
            Assert.Equal(filepath, profileImage.FilePath);
            Assert.Equal(Guid.Empty, profileImage.Id);
            Assert.Equal(Guid.Empty, profileImage.UserId);
        }

        [Fact]
        public void ShouldConstructWithIdFilenameAndFilepathConstructor()
        {
            Guid id = Guid.NewGuid();
            string filename = "FILENAME";
            string filepath = "FILEPATH";
            ProfileImage profileImage = new ProfileImage(id, filename, filepath);

            Assert.Null(profileImage.User);
            Assert.Equal(filename, profileImage.FileName);
            Assert.Equal(filepath, profileImage.FilePath);
            Assert.Equal(id, profileImage.Id);
            Assert.Equal(Guid.Empty, profileImage.UserId);
        }
    }
}
