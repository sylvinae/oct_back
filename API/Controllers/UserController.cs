using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(UserService user, ILogger<UserController> log, AuthService auth)
        : ControllerBase
    {
        private readonly UserService _user = user;
        private readonly ILogger<UserController> _log = log;
        private readonly AuthService _auth = auth;

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDto registerDetails)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userRole = _auth.GetUserRole();
                var result = await _user.CreateUserAsync(registerDetails);

                if (result == null)
                    return Conflict($"User with email {registerDetails.Email} already exists");

                var newUser = new ReturnDto
                {
                    Id = result.Id,
                    Email = result.Email,
                    Role = result.Role,

                    FirstName = result.FirstName,
                    MiddleName = result.MiddleName,
                    LastName = result.LastName,
                };

                return CreatedAtAction(nameof(RegisterUser), new { id = result.Id }, newUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error creating user: {ex}" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto u)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (user, jwt) = await _user.LoginAsync(u.Email, u.Password);
            if (user != null)
            {
                user.Password = "";
            }

            return user == null
                ? Unauthorized(new { message = "Wrong email/password" })
                : Ok(new { token = jwt, userDetails = user });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var result = await _user.DeleteUserAsync(id);

                return result ? Ok() : NotFound(new { message = "User does not exist." });
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPatch("undelete/{id}")]
        public async Task<IActionResult> UndeleteUser(Guid id)
        {
            try
            {
                var result = await _user.UndeleteUserAsync(id);

                return result ? Ok() : NotFound(new { message = "User does not exist." });
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}
