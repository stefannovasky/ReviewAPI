using System;
using ReviewApi.Shared.Utils;

namespace ReviewApi.IntegrationTests.Helpers
{
    internal class AuthorizationTokenHelper
    {
        public string CreateToken(Guid userId)
        {
            string token = new JwtTokenUtils("ajsdhausd62313gshaJJJ").GenerateToken(userId.ToString());
            return $"Bearer {token}";
        }
    }
}
