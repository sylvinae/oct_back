using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using API.Utils;
using Newtonsoft.Json;

//excuse the mess.
//to be cleaned

namespace API.DTOs
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    public class LoginDto
    {
        [JsonProperty("email")]
        [EmailAddress]
        public required string Email { get; set; }

        [JsonProperty("password")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }

    public class ReturnDto
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("email")]
        [EmailAddress]
        public required string Email { get; set; }

        [JsonProperty("role")]
        public required string Role { get; set; }

        [JsonProperty("firstName")]
        public required string FirstName { get; set; }

        [JsonProperty("middleName")]
        [JsonIgnoreIfEmpty]
        public string? MiddleName { get; set; }

        [JsonProperty("lastName")]
        public required string LastName { get; set; }

        [JsonProperty("jwt")]
        [JsonIgnoreIfEmpty]
        public string? Jwt { get; set; }
    }

    public class RegisterDto
    {
        [JsonProperty("firstName")]
        public required string FirstName { get; set; }

        [JsonProperty("middleName")]
        [JsonIgnoreIfEmpty]
        public string? MiddleName { get; set; }

        [JsonProperty("lastName")]
        public required string LastName { get; set; }

        [JsonProperty("role")]
        [RoleValidator(["superadmin", "admin", "user"])]
        public required string Role { get; set; }

        [JsonProperty("email")]
        [EmailAddress]
        public required string Email { get; set; }

        [JsonProperty("password")]
        [JsonIgnoreIfEmpty]
        public required string Password { get; set; }
    }

    public class LoginResponseDto
    {
        [JsonProperty("email")]
        [EmailAddress]
        public required string Email { get; set; }

        [JsonProperty("jwt")]
        [JsonIgnoreIfEmpty]
        public string? Jwt { get; set; }

        [JsonProperty("firstName")]
        public required string FirstName { get; set; }

        [JsonProperty("middleName")]
        public string? MiddleName { get; set; }

        [JsonProperty("lastName")]
        public required string LastName { get; set; }

        [JsonProperty("role")]
        public required string Role { get; set; }
    }
}
