using Microsoft.Data.SqlClient;
using MinesweeperGame.Models;
using System.Security.Cryptography;
using System.Text;

namespace MinesweeperGame.Data
{
    public class UserRepository
    {
        // String to connect to the database
        private readonly string _connectionString;

        // Constructor to help with the configuration of the injection
        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MinesweeperDB");
        }

        // Method to register a new user in the database
        public int RegisterUser(User user)
        {
                // Generate salt and hash the password
                var salt = GenerateSalt();
                var passwordHash = HashPassword(user.Password, salt);

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                // SQL command to insert user and return new ID
                    var command = new SqlCommand(
                        "INSERT INTO Users (FirstName, LastName, Sex, Age, State, Email, Username, PasswordHash, Salt) " +
                        "VALUES (@FirstName, @LastName, @Sex, @Age, @State, @Email, @Username, @PasswordHash, @Salt); " +
                        "SELECT CAST(SCOPE_IDENTITY() as int)", connection);
                    
                    // Add parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);
                    command.Parameters.AddWithValue("@LastName", user.LastName);
                    command.Parameters.AddWithValue("@Sex", user.Sex);
                    command.Parameters.AddWithValue("@Age", user.Age);
                    command.Parameters.AddWithValue("@State", user.State);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    command.Parameters.AddWithValue("@Salt", salt);
                    
                    // Return the new user ID
                    return (int)command.ExecuteScalar();
                }
        }

        // Method to help authenticate the user
        public User AuthenticateUser(string username, string password)
        {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    // SQL command to get the user based on there username
                    var command = new SqlCommand(
                        "SELECT Id, Username, PasswordHash, Salt FROM Users WHERE Username = @Username",
                        connection);

                    command.Parameters.AddWithValue("@Username", username);

                    using (var reader = command.ExecuteReader())
                    {
                        // if the user if found
                        if (reader.Read())
                        {
                            // Get stored hash
                            var storedHash = reader.GetString(2);
                            // Get the salt
                            var salt = reader.GetString(3);
                            // Hash provided password
                            var inputHash = HashPassword(password, salt);
                            
                            // Compare hashes
                            if (storedHash == inputHash)
                            {
                                // Return user info if a match is found
                                return new User
                                {
                                    Id = reader.GetInt32(0),
                                    Username = reader.GetString(1)
                                };
                            }
                        }
                    }
                    // Return null if suthentication fails
                    return null;
                }
        }

        // Method to generate a random salt for password hashing
        private string GenerateSalt()
        {
            var bytes = new byte[16];
            using (var random = RandomNumberGenerator.Create())
            {
                // Fill with random bytes
                random.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }

        // Method to hash a password with the salt
        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}