using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API.Models;
using Microsoft.IdentityModel.Tokens;

namespace API.Utils
{
    public class Cryptics
    {
        public static string ComputeHash(object itemDto)
        {
            var propertiesToInclude = new[]
            {
                "Barcode",
                "Brand",
                "Generic",
                "Classification",
                "Formulation",
                "Location",
                "Wholesale",
                "Retail",
                "Company",
                "Expiry",
                "IsReagent",
            };

            var propertyValues = itemDto
                .GetType()
                .GetProperties()
                .Where(p => propertiesToInclude.Contains(p.Name))
                .Select(p => p.GetValue(itemDto)?.ToString() ?? string.Empty);

            var input = string.Join("-", propertyValues);
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        public static string GenerateJwtToken(UserModel user)
        {
            var key = Encoding.ASCII.GetBytes(
                DotNetEnv.Env.GetString("JWT_SECRET") ?? "default_strengamabobs"
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim("UserId", user.Id.ToString()),
                    ]
                ),
                Expires = DateTime.UtcNow.AddHours(10),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Issuer = DotNetEnv.Env.GetString("JWT_ISSUER"),
                Audience = DotNetEnv.Env.GetString("JWT_AUDIENCE"),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
