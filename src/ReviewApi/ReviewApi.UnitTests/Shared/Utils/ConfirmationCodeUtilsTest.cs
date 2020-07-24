using ReviewApi.Shared.Interfaces;
using ReviewApi.Shared.Utils;
using Xunit;

namespace ReviewApi.UnitTests.Shared.Utils
{
    public class ConfirmationCodeUtilsTest
    {
        private readonly IConfirmationCodeUtils _confirmationCodeUtils;
        public ConfirmationCodeUtilsTest()
        {
            _confirmationCodeUtils = new ConfirmationCodeUtils();
        }

        [Fact]
        public void ShouldGenerateRandomConfirmationCode()
        {
            string confirmationCode = _confirmationCodeUtils.GenerateConfirmationCode();
            Assert.NotNull(confirmationCode);
            Assert.Equal(8, confirmationCode.Length);
        }
    }
}
