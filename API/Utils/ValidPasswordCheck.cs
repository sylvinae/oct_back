using System.ComponentModel.DataAnnotations;

namespace API.Utils
{
    public class PasswordValidatorAttribute : ValidationAttribute
    {
        public int MinimumLength { get; set; } = 6;

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            if (value is not string password)
            {
                return new ValidationResult("Password cannot be null");
            }

            var errorMessages = new List<string>();

            if (password.Length < MinimumLength)
                errorMessages.Add($"be at least {MinimumLength} characters long");

            if (!password.Any(char.IsDigit))
                errorMessages.Add("contain at least one number");

            if (!password.Any(char.IsUpper))
                errorMessages.Add("contain at least one uppercase letter");

            if (!password.Any(char.IsLower))
                errorMessages.Add("contain at least one lowercase letter");

            if (errorMessages.Count != 0)
            {
                var combinedErrors = $"Password must {string.Join(", ", errorMessages)}.";
                return new ValidationResult(combinedErrors);
            }

            return ValidationResult.Success;
        }
    }
}
