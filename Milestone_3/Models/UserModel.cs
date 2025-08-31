using System.Security.Cryptography;
using System.Text;

namespace MinesweeperGame.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public int Age { get; set; }
        public string State { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public byte[] Salt { get; set; }

        public void SetPassword(string password)
        {
            // Creating a random salt
            Salt = new byte[16];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(Salt);
            }

            // Combining the password and salt to create a hash
            var saltedPassword = Encoding.UTF8.GetBytes(password).Concat(Salt).ToArray();

            // Create hash using SHA256 algorithm
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(saltedPassword);
                PasswordHash = Convert.ToBase64String(hashBytes);
            }
        }

        public bool VerifyPassword(string password)
        {
            // Combining the password with stored salt
            var saltedPassword = Encoding.UTF8.GetBytes(password).Concat(Salt).ToArray();

            // Create hash to compare
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(saltedPassword);
                string hashVerify = Convert.ToBase64String(hashBytes);
                return hashVerify == PasswordHash;
            }
        }

    }
}
