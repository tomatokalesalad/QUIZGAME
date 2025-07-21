using System;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace QuizGame1WPF
{
    public class DatabaseHelper
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["QuizGameDB"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        // Validate user login
        public static bool ValidateUser(string username, string passwordHash)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        // Register new user
        public static bool RegisterUser(string username, string passwordHash)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();

                // Check if username already exists
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@Username", username);
                int userExists = (int)checkCmd.ExecuteScalar();
                if (userExists > 0)
                    return false;

                // Insert new user
                string insertQuery = "INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash)";
                SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@Username", username);
                insertCmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                insertCmd.ExecuteNonQuery();
                return true;
            }
        }
    }
}
