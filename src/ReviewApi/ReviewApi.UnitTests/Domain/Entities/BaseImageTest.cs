using System;
using ReviewApi.Domain.Entities;
using Xunit;

namespace ReviewApi.UnitTests.Domain.Entities
{
    public class BaseImageTest
    {
        [Fact]
        public void ShouldConstructWithParameterlessConstructor()
        {
            BaseImage image = new BaseImage();

            Assert.Null(image.FileName);
            Assert.Null(image.FilePath);
            Assert.Equal(Guid.Empty, image.Id);
        }

        [Fact]
        public void ShouldConstructWithOnlyIdConstructor()
        {
            Guid id = Guid.NewGuid();
            BaseImage image = new BaseImage(id);

            Assert.Null(image.FileName);
            Assert.Null(image.FilePath);
            Assert.Equal(id, image.Id);
        }

        [Fact]
        public void ShouldConstructWithFileInfosParamsConstructor()
        {
            string fileName = "fileName";
            string filePath = "filePath";
            BaseImage image = new BaseImage(fileName, filePath);

            Assert.Equal(fileName, image.FileName);
            Assert.Equal(filePath, image.FilePath);
            Assert.Equal(Guid.Empty, image.Id);
        }

        [Fact]
        public void ShouldConstructWithIdAndFileInfosParamsConstructor()
        {
            Guid id = Guid.NewGuid();
            string fileName = "fileName";
            string filePath = "filePath";
            BaseImage image = new BaseImage(id, fileName, filePath);

            Assert.Equal(fileName, image.FileName);
            Assert.Equal(filePath, image.FilePath);
            Assert.Equal(id, image.Id);
        }
    }
}
