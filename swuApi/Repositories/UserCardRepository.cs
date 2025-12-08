using Microsoft.Data.SqlClient;
using swuApi.Models;
using System.Data.Common;

namespace swuApi.Repositories
{
    public class UserCardRepository : IUserCardRepository
    {
        private readonly string _connectionString;

        private const string UserCardBaseQuery =
            "SELECT Id, UserId, CardId, Copies, DateAdded, IsFavorite FROM UserCards";

        public UserCardRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private UserCard MapToUserCard(DbDataReader reader)
        {
            return new UserCard
            {
                Id = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                CardId = reader.GetInt32(2),
                Copies = reader.GetInt32(3),
                DateAdded = reader.GetDateTime(4),
                IsFavorite = reader.GetBoolean(5)
            };
        }

        public async Task<List<UserCard>> GetAllAsync()
        {
            var userCards = new List<UserCard>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(UserCardBaseQuery, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                userCards.Add(MapToUserCard(reader));
            }
            return userCards;
        }

        public async Task<List<UserCard>> GetFilteredAsync(string? filterField, string? filterValue, string? sortField, string? sortDirection)
        {
            var userCards = new List<UserCard>();

            // Campos permitidos para filtrar y ordenar (evitar SQL injection)
            var allowedFilterFields = new HashSet<string> { "UserId", "CardId", "Copies", "IsFavorite" };
            var allowedSortFields = new HashSet<string> { "Id", "UserId", "CardId", "Copies", "DateAdded", "IsFavorite" };

            var query = UserCardBaseQuery;
            var parameters = new List<SqlParameter>();

            // Construir cláusula WHERE si filterField y filterValue son válidos
            if (!string.IsNullOrWhiteSpace(filterField) && allowedFilterFields.Contains(filterField) && !string.IsNullOrWhiteSpace(filterValue))
            {
                query += $" WHERE {filterField} = @FilterValue";
                parameters.Add(new SqlParameter("@FilterValue", filterValue));
            }

            // Ordenamiento seguro
            if (!string.IsNullOrWhiteSpace(sortField) && allowedSortFields.Contains(sortField))
            {
                var direction = sortDirection?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true ? "DESC" : "ASC";
                query += $" ORDER BY {sortField} {direction}";
            }

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            if (parameters.Count > 0)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                userCards.Add(MapToUserCard(reader));
            }

            return userCards;
        }

        public async Task<UserCard?> GetByIdAsync(int id)
        {
            UserCard? userCard = null;
            string query = $"{UserCardBaseQuery} WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                userCard = MapToUserCard(reader);
            }
            return userCard;
        }

        public async Task AddAsync(UserCard entity)
        {
            string query = @"
                INSERT INTO UserCards (UserId, CardId, Copies, DateAdded, IsFavorite) 
                OUTPUT INSERTED.Id 
                VALUES (@UserId, @CardId, @Copies, @DateAdded, @IsFavorite)";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserId", entity.UserId);
            command.Parameters.AddWithValue("@CardId", entity.CardId);
            command.Parameters.AddWithValue("@Copies", entity.Copies);
            command.Parameters.AddWithValue("@DateAdded", entity.DateAdded == default ? DateTime.UtcNow : entity.DateAdded);
            command.Parameters.AddWithValue("@IsFavorite", entity.IsFavorite);

            await connection.OpenAsync();
            entity.Id = (int)await command.ExecuteScalarAsync();
        }

        public async Task UpdateAsync(UserCard entity)
        {
            string query = @"
                UPDATE UserCards 
                SET Copies=@Copies, DateAdded=@DateAdded, IsFavorite=@IsFavorite 
                WHERE Id=@Id";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", entity.Id);
            command.Parameters.AddWithValue("@Copies", entity.Copies);
            command.Parameters.AddWithValue("@DateAdded", entity.DateAdded == default ? DateTime.UtcNow : entity.DateAdded);
            command.Parameters.AddWithValue("@IsFavorite", entity.IsFavorite);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            string query = "DELETE FROM UserCards WHERE Id = @Id";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<UserCard?> GetByUserIdAndCardIdAsync(int userId, int cardId)
        {
            string query = $"{UserCardBaseQuery} WHERE UserId = @UserId AND CardId = @CardId";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@CardId", cardId);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapToUserCard(reader);
            }
            return null;
        }

        public async Task UpsertCopiesAsync(int userId, int cardId, int copiesToAdd)
        {
            string query = @"
                IF EXISTS (SELECT 1 FROM UserCards WHERE UserId = @UserId AND CardId = @CardId)
                BEGIN
                    UPDATE UserCards 
                    SET Copies = Copies + @CopiesToAdd, DateAdded = GETUTCDATE()
                    WHERE UserId = @UserId AND CardId = @CardId
                END
                ELSE
                BEGIN
                    INSERT INTO UserCards (UserId, CardId, Copies, DateAdded, IsFavorite)
                    VALUES (@UserId, @CardId, @CopiesToAdd, GETUTCDATE(), 0)
                END";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@CardId", cardId);
            command.Parameters.AddWithValue("@CopiesToAdd", copiesToAdd);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<UserCard>> GetInventoryByUserIdAsync(int userId, string? sortField, string? sortDirection)
        {
            var userCards = new List<UserCard>();

            var allowedSortFields = new HashSet<string> { "Id", "UserId", "CardId", "Copies", "DateAdded", "IsFavorite" };

            string query = $"{UserCardBaseQuery} WHERE UserId = @UserId";
            if (!string.IsNullOrWhiteSpace(sortField) && allowedSortFields.Contains(sortField))
            {
                var direction = sortDirection?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true ? "DESC" : "ASC";
                query += $" ORDER BY {sortField} {direction}";
            }

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserId", userId);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                userCards.Add(MapToUserCard(reader));
            }
            return userCards;
        }
    }
}