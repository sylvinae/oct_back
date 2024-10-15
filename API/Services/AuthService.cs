namespace API.Services
{
    public class AuthService(IHttpContextAccessor httpContextAccessor)
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public string? GetUserRole()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var roleClaim = httpContext?.User?.FindFirst("role");

            return roleClaim?.Value;
        }

        public bool HasPermission()
        {
            var userRole = GetUserRole();
            return userRole != null
                && (
                    userRole.Equals("admin", StringComparison.OrdinalIgnoreCase)
                    || userRole.Equals("superadmin", StringComparison.OrdinalIgnoreCase)
                );
        }

        public bool HasAnyPermission(string[] validRoles)
        {
            var userRole = GetUserRole();
            return userRole != null
                && validRoles.Contains(userRole, StringComparer.OrdinalIgnoreCase);
        }
    }
}
