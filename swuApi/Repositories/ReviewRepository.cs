using Microsoft.Data.SqlClient;
using swuApi.Models;
using swuApi.Enums;
using System.Data.Common;

namespace swuApi.Repositories
{
    public class ReviewRepository : IRepository<Review>
    {
        private readonly string _connectionString;

        public ReviewRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private static readonly HashSet<string> ValidFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "CreationDate", "MessageReview", "Stars"
        };

        private Review MapToReview(DbDataReader reader)
        {
            return new Review
            {
                Id = reader.GetInt32(0),
                CreationDate = reader.GetDateTime(1),
                MessageReview = reader.GetString(2),
                Stars = (ReviewValueType)reader.GetInt32(3),
                UserId = reader.GetInt32(4)
            };
        }

        // GetFilteredAsync
        public async Task<List<Review>> GetFilteredAsync(string? filterField, string? filterValue, string? sortField, string? sortDirection)
        {
            var reviews = new List<Review>();

            var baseQuery = @"
                SELECT r.Id, r.CreationDate, r.MessageReview, r.Stars, r.UserId
                FROM Reviews r";

            var whereClause = "";
            var orderByClause = "";
            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(filterField) &&
                !string.IsNullOrWhiteSpace(filterValue) &&
                ValidFields.Contains(filterField))
            {
                whereClause = $" WHERE r.{filterField} LIKE @FilterValue";
                parameters.Add("@FilterValue", $"%{filterValue}%");
            }

            if (!string.IsNullOrWhiteSpace(sortField) &&
                ValidFields.Contains(sortField))
            {
                var direction = sortDirection?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true
                    ? "DESC"
                    : "ASC";

                orderByClause = $" ORDER BY r.{sortField} {direction}";
            }

            string finalQuery = baseQuery + whereClause + orderByClause;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(finalQuery, connection))
                {
                    foreach (var param in parameters)
                        command.Parameters.AddWithValue(param.Key, param.Value);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            reviews.Add(MapToReview(reader));
                        }
                    }
                }
            }

            return reviews;
        }

        // GetAllAsync
        public async Task<List<Review>> GetAllAsync()
        {
            var reviews = new List<Review>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT Id, CreationDate, MessageReview, Stars, UserId FROM Reviews";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        reviews.Add(MapToReview(reader));
                    }
                }
            }

            return reviews;
        }

        // GetByIdAsync
        public async Task<Review?> GetByIdAsync(int id)
        {
            Review? review = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT Id, CreationDate, MessageReview, Stars, UserId
                    FROM Reviews WHERE Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            review = MapToReview(reader);
                        }
                    }
                }
            }

            return review;
        }

        // GetAllReviewsInUserAsync
        public async Task<List<Review>> GetAllReviewsInUserAsync(int userId)
        {
            var reviews = new List<Review>();
            string query = "SELECT Id, CreationDate, MessageReview, Stars, UserId FROM Reviews WHERE UserId = @UserId";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        reviews.Add(MapToReview(reader));
                    }
                }
            }

            return reviews;
        }

        // AddAsync: POST Review
        public async Task AddAsync(Review review)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = @"
                INSERT INTO Reviews (CreationDate, MessageReview, Stars, UserId)
                VALUES (@CreationDate, @MessageReview, @Stars, @UserId);
                SELECT SCOPE_IDENTITY();";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CreationDate", review.CreationDate);
            command.Parameters.AddWithValue("@MessageReview", review.MessageReview);
            command.Parameters.AddWithValue("@Stars", review.Stars);
            command.Parameters.AddWithValue("@UserId", review.UserId);

            var newId = await command.ExecuteScalarAsync();

            if (newId != null && newId != DBNull.Value)
            {
                review.Id = Convert.ToInt32(newId);
            }
        }

        // UpdateAsync: PUT Review
        public async Task UpdateAsync(Review review)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    UPDATE Reviews 
                    SET CreationDate = @CreationDate, MessageReview = @MessageReview, Stars = @Stars, UserId = @UserId 
                    WHERE Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", review.Id);
                    command.Parameters.AddWithValue("@CreationDate", review.CreationDate);
                    command.Parameters.AddWithValue("@MessageReview", review.MessageReview);
                    command.Parameters.AddWithValue("@Stars", review.Stars);
                    command.Parameters.AddWithValue("@UserId", review.UserId);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // DeleteAsync: DELETE Review
        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "DELETE FROM Reviews WHERE Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
