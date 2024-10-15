using System.ComponentModel.DataAnnotations;

namespace API.Utils
{
    public class RoleValidatorAttribute(string[] validRoles) : ValidationAttribute
    {
        private readonly string[] _validRoles = validRoles;

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            if (value is string role)
            {
                if (_validRoles.Contains(role.ToLower()))
                {
                    return ValidationResult.Success;
                }
            }
            string validRolesList = string.Join(", ", _validRoles);
            return new ValidationResult(
                $"Invalid role provided. Must be one of: {validRolesList}."
            );
        }
    }
}
