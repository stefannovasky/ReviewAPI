using System;
using ReviewApi.Domain.Entities;
using Xunit;

namespace ReviewApi.UnitTests.Domain.Entities
{
    public class ImageTest
    {
        [Fact]
        public void ShouldConstructWithParameterlessConstructor()
        {
            Image image = new Image();

            Assert.Null(image.FileName);
            Assert.Null(image.FilePath);
            Assert.Null(image.User);
            Assert.Equal(Guid.Empty, image.Id);
            Assert.Equal(Guid.Empty, image.UserId);
        }

        [Fact]
        public void ShouldConstructWithOnlyIdConstructor()
        {
            Guid id = Guid.NewGuid();
            Image image = new Image(id);

            Assert.Null(image.FileName);
            Assert.Null(image.FilePath);
            Assert.Null(image.User);
            Assert.Equal(id, image.Id);
            Assert.Equal(Guid.Empty, image.UserId);
        }

        [Fact]
        public void ShouldConstructWithFileInfosParamsConstructor()
        {
            string fileName = "fileName";
            string filePath = "filePath";
            Image image = new Image(fileName, filePath);

            Assert.Equal(fileName, image.FileName);
            Assert.Equal(filePath, image.FilePath);
            Assert.Equal(Guid.Empty, image.Id);
            Assert.Equal(Guid.Empty, image.UserId);
            Assert.Null(image.User);
        }

        [Fact]
        public void ShouldConstructWithIdAndFileInfosParamsConstructor()
        {
            Guid id = Guid.NewGuid();
            string fileName = "fileName";
            string filePath = "filePath";
            Image image = new Image(id, fileName, filePath);

            Assert.Equal(fileName, image.FileName);
            Assert.Equal(filePath, image.FilePath);
            Assert.Equal(id, image.Id);
            Assert.Equal(Guid.Empty, image.UserId);
            Assert.Null(image.User);
        }

        [Fact]
        public void ShouldConstructWithAllParamsConstructor()
        {
            Guid imageId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            string fileName = "fileName";
            string filePath = "filePath";
            Image image = new Image(imageId, fileName, filePath, userId);

            Assert.Equal(fileName, image.FileName);
            Assert.Equal(filePath, image.FilePath);
            Assert.Equal(imageId, image.Id);
            Assert.Equal(userId, image.UserId);
            Assert.Null(image.User);
        }

        [Fact]
        public void ShouldConstructNoImageIdParamsConstructor()
        {
            Guid userId = Guid.NewGuid();
            string fileName = "fileName";
            string filePath = "filePath";
            Image image = new Image(fileName, filePath, userId);

            Assert.Equal(fileName, image.FileName);
            Assert.Equal(filePath, image.FilePath);
            Assert.Equal(Guid.Empty, image.Id);
            Assert.Equal(userId, image.UserId);
            Assert.Null(image.User);
        }
    }
}
