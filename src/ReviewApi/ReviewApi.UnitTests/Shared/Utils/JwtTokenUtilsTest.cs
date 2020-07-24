using ReviewApi.Shared.Interfaces;
using ReviewApi.Shared.Utils;
using Xunit;

namespace ReviewApi.UnitTests.Shared.Utils
{
    public class JwtTokenUtilsTest
    {
        private readonly IJwtTokenUtils _jwtTokenUtils;

        public JwtTokenUtilsTest()
        {
            _jwtTokenUtils = new JwtTokenUtils("secret_key_secret_key_secret_key");
        }

        [Fact]
        public void ShouldGenerateToken()
        {
            string token = _jwtTokenUtils.GenerateToken("user id");
            Assert.True(token.Length > 100);
        }
    }
}
