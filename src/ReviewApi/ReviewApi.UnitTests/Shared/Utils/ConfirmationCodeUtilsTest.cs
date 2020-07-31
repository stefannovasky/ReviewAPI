using ReviewApi.Shared.Interfaces;
using ReviewApi.Shared.Utils;
using Xunit;

namespace ReviewApi.UnitTests.Shared.Utils
{
    public class RandomCodeUtilsTest
    {
        private readonly IRandomCodeUtils _randomCodeUtils;
        public RandomCodeUtilsTest()
        {
            _randomCodeUtils = new RandomCodeUtils();
        }

        [Fact]
        public void ShouldGenerateRandomConfirmationCode()
        {
            string confirmationCode = _randomCodeUtils.GenerateRandomCode();
            Assert.NotNull(confirmationCode);
            Assert.Equal(8, confirmationCode.Length);
        }
    }
}
