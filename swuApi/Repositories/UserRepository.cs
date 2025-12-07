using Microsoft.Data.SqlClient;
using swuApi.Models;
using System.Data.Common;

namespace swuApi.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly string _connectionString;

        public UserRepository (string connectionString)
        {
            _connectionString = connectionString;
        }

        private static readonly HashSet<String> ValidFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Username", "Email", "RegistrationDate", "IsActive", "TotalCollectionValue"
        };

        private User MapToUser(DbDataReader reader)
        {
            return new User
            {
              Id = reader.GetInt32(0),
              Username = reader.GetString(1),
              Email = reader.GetString(2),
              PasswordHash = reader.GetString(3),
              RegistrationDate = reader.GetDateTime(4),
              IsActive = reader.GetBoolean(5),
              TotalCollectionValue = reader.GetDecimal(6),
            };
        }

        private const string UserBaseQuery = 
            "SELECT Id, Username, Email, PasswordHash, RegistrationDate, IsActive, TotalCollectionValue FROM Users";

        public async Task<List<User>> GetAllAsync()
        {
            var users = new List<User>();
            
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(UserBaseQuery, connection))
            {
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(MapToUser(reader));
                    }
                }
            }
            return users;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            User? user = null;
            string query = $"{UserBaseQuery} WHERE Id = @Id";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        user = MapToUser(reader);
                    }
                }
            }
            return user;
        }
        
        // Usuarios filtrados y ordenados
        public async Task<List<User>> GetFilteredAsync(string? filterField, string? filterValue, string? sortField, string? sortDirection)
        {
             var users = new List<User>();
            
            var baseQuery = UserBaseQuery;
            var whereClause = "";
            var orderByClause = "";
            var parameters = new Dictionary<string, object>();
            
            // Lógica de filtrado
            if (!string.IsNullOrWhiteSpace(filterField) && !string.IsNullOrWhiteSpace(filterValue) && ValidFields.Contains(filterField))
            {
                whereClause = $" WHERE {filterField} LIKE @FilterValue";
                parameters.Add("@FilterValue", $"%{filterValue}%");
            }

            // Lógica de ordenación
            if (!string.IsNullOrWhiteSpace(sortField) && ValidFields.Contains(sortField))
            {
                var direction = "ASC";
                if (sortDirection != null && sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    direction = "DESC";
                }
                orderByClause = $" ORDER BY {sortField} {direction}";
            }

            string finalQuery = baseQuery + whereClause + orderByClause;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(finalQuery, connection))
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value);
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(MapToUser(reader));
                        }
                    }
                }
            }
            return users;
        }

        public async Task<int> GetMaxIdAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            string query = "SELECT ISNULL(MAX(Id), 0) FROM Users";
            using var command = new SqlCommand(query, connection);
            var result = await command.ExecuteScalarAsync();
            return (int)result;
        }
        
        public async Task AddAsync(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = @"
                INSERT INTO Users (Username, Email, PasswordHash, RegistrationDate, IsActive, TotalCollectionValue)
                OUTPUT INSERTED.Id
                VALUES (@Username, @Email, @PasswordHash, @RegistrationDate, @IsActive, @TotalCollectionValue)";

            using var command = new SqlCommand(query, connection);

            // Mapeo de parámetros
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@RegistrationDate", user.RegistrationDate);
            command.Parameters.AddWithValue("@IsActive", user.IsActive);
            command.Parameters.AddWithValue("@TotalCollectionValue", user.TotalCollectionValue);

            // Ejecutamos y recuperamos el nuevo ID generado
            user.Id = (int)await command.ExecuteScalarAsync();
        }

        public async Task UpdateAsync(User user)
        {
            string query = @"
                UPDATE Users SET 
                    Username=@Username, 
                    Email=@Email, 
                    PasswordHash=@PasswordHash, 
                    RegistrationDate=@RegistrationDate, 
                    IsActive=@IsActive, 
                    TotalCollectionValue=@TotalCollectionValue 
                WHERE Id=@Id";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                // Mapeo de parámetros
                command.Parameters.AddWithValue("@Id", user.Id);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@RegistrationDate", user.RegistrationDate);
                command.Parameters.AddWithValue("@IsActive", user.IsActive);
                command.Parameters.AddWithValue("@TotalCollectionValue", user.TotalCollectionValue);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            string query = "DELETE FROM Users WHERE Id = @Id";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}