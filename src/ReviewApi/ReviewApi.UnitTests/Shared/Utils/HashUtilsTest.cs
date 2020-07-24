using ReviewApi.Shared.Interfaces;
using ReviewApi.Shared.Utils;
using Xunit;

namespace ReviewApi.UnitTests.Shared.Utils
{
    public class HashUtilsTest
    {
        private readonly IHashUtils _hashUtils;

        public HashUtilsTest()
        {
            _hashUtils = new HashUtils();
        }

        [Fact]
        public void ShouldHashString()
        {
            string plaintext = "text";

            string hash = _hashUtils.GenerateHash(plaintext);

            Assert.NotEqual(plaintext, hash);
        }

        [Fact]
        public void ShouldReturnTrueOnCompareHash() 
        {
            string plaintext = "text";

            string hash = _hashUtils.GenerateHash(plaintext);
            bool result = _hashUtils.CompareHash(plaintext, hash);

            Assert.True(result);
        }

        [Fact]
        public void ShouldReturnFalseOnCompareHash()
        {
            string plaintext = "text";

            string hash = _hashUtils.GenerateHash(plaintext);
            bool result = _hashUtils.CompareHash("not equal", hash);

            Assert.False(result);
        }
    }
}
