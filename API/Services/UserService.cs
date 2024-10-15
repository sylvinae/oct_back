using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Data;
using API.DTOs;
using API.Extensions;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class UserService(ApiDbContext db, ILogger<UserService> log)
    {
        private readonly ApiDbContext _db = db;
        private readonly ILogger<UserService> _log = log;

        public async Task<UserModel?> CreateUserAsync(RegisterDto u)
        {
            try
            {
                var exists = await _db.Users.AnyAsync(uu => uu.Email == u.Email);
                if (exists)
                {
                    _log.LogError("Failed to create user: user already exists");
                    return null;
                }

                var userModel = new UserModel
                {
                    FirstName = u.FirstName,
                    MiddleName = u.MiddleName ?? "",
                    LastName = u.LastName,
                    Email = u.Email,
                    Password = u.Password.HashPassword(),
                    Role = u.Role,
                };

                var user = await _db.Users.AddAsync(userModel);
                await _db.SaveChangesAsync();
                _log.LogInformation(
                    "User {}, {} has registered with email {}",
                    user.Entity.FirstName,
                    user.Entity.LastName,
                    user.Entity.Email
                );

                return user.Entity;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error creating new user.");
                throw;
            }
        }

        public async Task<(UserModel? model, string? jwt)> LoginAsync(string email, string password)
        {
            var hashedPass = password.HashPassword();
            _log.LogInformation("hashedpass:{}", hashedPass);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return (null, null);

            if (!password.Verify(hashedPass))
                return (null, null);

            var jwt = GenerateJwtToken(user);
            return (user, jwt);
        }

        public async Task<bool> DeleteUserAsync(Guid UserId)
        {
            var user = await _db.Users.FindAsync(UserId);
            if (user == null)
                return false;

            user.IsDeleted = true;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UndeleteUserAsync(Guid UserId)
        {
            var user = await _db.Users.FindAsync(UserId);
            if (user == null)
                return false;

            user.IsDeleted = false;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<UserModel?> GetUserAsync(Guid UserId)
        {
            return await _db.Users.FindAsync(UserId);
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
