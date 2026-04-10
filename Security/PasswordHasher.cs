namespace HospitalEHR.Security
{
    /// <summary>BCrypt one-way password hashing — stored hash cannot be reversed.</summary>
    public static class PasswordHasher
    {
    //controls hash complexity more value more security
        private const int WorkFactor = 12;

        public static string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));
            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
        }

        public static bool Verify(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
                return false;
            try   { return BCrypt.Net.BCrypt.Verify(password, hash); }
            catch (BCrypt.Net.SaltParseException) { return false; }
        }
    }
}
