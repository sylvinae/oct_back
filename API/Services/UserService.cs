using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Extensions;
using API.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class UserService(ApiDbContext context)
    {
        private readonly ApiDbContext _context = context;

        public async Task CreateUserAsync(UserModel userModel)
        {
            await _context.Users.AddAsync(userModel);
            await _context.SaveChangesAsync();
            return;
        }

        public async Task<UserModel?> LoginAsync(string email, string password)
        {
            var hashedPass = password.HashPassword();
            return await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == email && u.Password == password
            );
        }

        public async Task<bool> DeleteUserAsync(Guid UserId)
        {
            var user = await _context.Users.FindAsync(UserId);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserModel?> GetUserAsync(Guid UserId)
        {
            return await _context.Users.FindAsync(UserId);
        }

        private static string GenerateJwtToken(UserModel user)
        {
            var key = Encoding.ASCII.GetBytes(
                DotNetEnv.Env.GetString("JWT_SECRET") ?? "default_strengamabobs"
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    [new Claim(ClaimTypes.Name, user.Email), new Claim(ClaimTypes.Role, "User")]
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
