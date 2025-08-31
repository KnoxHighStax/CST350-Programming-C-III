using Microsoft.Data.SqlClient;

namespace MinesweeperGame.Models
{
    public class UserCollection
    {
        private readonly string _connectionString;

        public UserCollection()
        {
            _connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=MinesweeperDB;Trusted_Connection=True;";
        }

        public int AddUser(UserModel user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Users (FirstName, LastName, Sex, Age, State, Email, Username, PasswordHash, Salt) " +
                    "OUTPUT INSERTED.Id " +
                    "VALUES (@FirstName, @LastName, @Sex, @Age, @State, @Email, @Username, @PasswordHash, @Salt)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);
                    command.Parameters.AddWithValue("@LastName", user.LastName);
                    command.Parameters.AddWithValue("@Sex", user.Sex);
                    command.Parameters.AddWithValue("@Age", user.Age);
                    command.Parameters.AddWithValue("@State", user.State);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@Salt", user.Salt);

                    connection.Open();
                    int newId = (int)command.ExecuteScalar();
                    return newId;
                }
            }
        }

        public int CheckCredentials(string username, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT Id, PasswordHash, Salt FROM Users WHERE Username = @Username",
                    connection);
                command.Parameters.AddWithValue("@Username", username);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int userId = reader.GetInt32(0);
                        string storedHash = reader.GetString(1);
                        byte[] salt = (byte[])reader[2];

                        var newUser = new UserModel
                        {
                            PasswordHash = storedHash,
                            Salt = salt
                            
                        };

                        if (newUser.VerifyPassword(password))
                        {
                            return userId;
                        }
                    }
                }
            }
            return 0;
        }

        public UserModel GetUserByUsername(string username)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, FirstName, LastName, Sex, Age, State, Email, Username, PasswordHash, Salt " +
                               "FROM Users WHERE Username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserModel
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Sex = reader.GetString(3),
                                Age = reader.GetInt32(4),
                                State = reader.GetString(5),
                                Email = reader.GetString(6),
                                Username = reader.GetString(7),
                                PasswordHash = reader.GetString(8),
                                Salt = (byte[])reader[9]
                            };
                        }
                    }
                }
            }
            return null;
        }

        public UserModel GetUserById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, FirstName, LastName, Sex, Age, State, Email, Username, PasswordHash, Salt " +
                               "FROM Users WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserModel
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Sex = reader.GetString(3),
                                Age = reader.GetInt32(4),
                                State = reader.GetString(5),
                                Email = reader.GetString(6),
                                Username = reader.GetString(7),
                                PasswordHash = reader.GetString(8),
                                Salt = (byte[])reader[9]
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool DoesUsernameExists(string username)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public bool DoesEmailExists(string email)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
    }
}
