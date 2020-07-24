using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ReviewApi.Shared.Interfaces;

namespace ReviewApi.Shared.Utils
{
    public class JwtTokenUtils : IJwtTokenUtils
    {
        private readonly string _secretKey;

        public JwtTokenUtils(string secretKey)
        {
            _secretKey = secretKey;
        }

        public string GenerateToken(string id)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_secretKey);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, id),
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
