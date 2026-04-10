using HospitalEHR.Security;

namespace HospitalEHR.Extensions
{
    public static class SessionExtensions
    {
        public static int?    GetUserId  (this ISession s) => int.TryParse(s.GetString(SessionKeys.UserId), out var id) ? id : null;
        public static string? GetFullName(this ISession s) => s.GetString(SessionKeys.FullName);
        public static string? GetEmail   (this ISession s) => s.GetString(SessionKeys.Email);
        public static string? GetRole    (this ISession s) => s.GetString(SessionKeys.Role);
        public static bool IsAuthenticated(this ISession s) => s.GetUserId() != null;
        public static bool IsInRole(this ISession s, string role) =>
            string.Equals(s.GetRole(), role, StringComparison.OrdinalIgnoreCase);
        public static bool IsInRoles(this ISession s, params string[] roles) =>
            roles.Any(r => s.IsInRole(r));
    }
}
