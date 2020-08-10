using System.IO;

namespace ReviewApi.IntegrationTests.Helpers
{
    public class TestFileHelper
    {
        public byte[] GetTestFileImage()
        {
            string imageFilePath = Path.GetFullPath(Path.Combine(@"..\..\..\", "Files", "test-image.jpg"));
            return File.ReadAllBytes(imageFilePath);
        }
    }
}
