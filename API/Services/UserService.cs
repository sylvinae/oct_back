using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class UserService
    {
        private readonly ApiDbContext _context;

        public UserService(ApiDbContext context)
        {
            _context = context;
        }

        // Create a new user
        public async Task<UserModel> CreateUserAsync(UserModel userModel)
        {
            // Perform any necessary validation or checks here

            await _context.Users.AddAsync(userModel);
            await _context.SaveChangesAsync();
            return userModel;
        }

        // Login a user
        public async Task<UserModel?> LoginAsync(string email, string password)
        {
            // You might want to hash the password before comparing
            return await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == email && u.Password == password
            );
        }

        // Delete a user by ID
        public async Task<bool> DeleteUserAsync(Guid UserId)
        {
            var user = await _context.Users.FindAsync(UserId);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get a user by ID
        public async Task<UserModel?> GetUserAsync(Guid UserId)
        {
            return await _context.Users.FindAsync(UserId);
        }
    }
}
