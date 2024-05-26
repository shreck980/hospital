using BCrypt.Net;

namespace hospital.Entities
{
    public class PasswordManager
    {

        public static string HashPassword(string password)
        {
            //string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            return hashedPassword;
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {

            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
