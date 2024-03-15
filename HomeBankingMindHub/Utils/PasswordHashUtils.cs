namespace HomeBankingMindHub.Utils
{
    public class PasswordHashUtils
    {
        public static string HashPassword(string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            return passwordHash;
        }
    }
}
