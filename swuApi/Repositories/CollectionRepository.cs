using Microsoft.Data.SqlClient;
using swuApi.Models;

namespace swuApi.Repositories
{
    public class CollectionRepository : IRepository<Collection>
    {
        private readonly string _connectionString;

        private static readonly HashSet<string> ValidFields = new(StringComparer.OrdinalIgnoreCase)
        {
            "CollectionName", "Color", "NumCards", "EstimatedValue", "CreationDate", "IsComplete"
        };

        public CollectionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private Collection MapToCollection(SqlDataReader reader)
        {
            return new Collection
            {
                Id = reader.GetInt32(0),
                CollectionName = reader.GetString(1),
                Color = reader.IsDBNull(2) ? null : reader.GetString(2),
                NumCards = reader.GetInt32(3),
                EstimatedValue = reader.GetDecimal(4),
                CreationDate = reader.GetDateTime(5),
                IsComplete = reader.GetBoolean(6)
            };
        }

        // GET All
        public async Task<List<Collection>> GetAllAsync()
        {
            var collections = new List<Collection>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = @"
                SELECT Id, CollectionName, Color, NumCards, EstimatedValue, CreationDate, IsComplete 
                FROM Collections";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                collections.Add(MapToCollection(reader));
            }

            return collections;
        }

        // GET By Id
        public async Task<Collection?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = @"
                SELECT Id, CollectionName, Color, NumCards, EstimatedValue, CreationDate, IsComplete 
                FROM Collections WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToCollection(reader);
            }

            return null;
        }

        // GET Filtered
        public async Task<List<Collection>> GetFilteredAsync(string? filterField, string? filterValue, string? sortField, string? sortDirection)
        {
            var collections = new List<Collection>();
            var whereClause = "";
            var orderByClause = "";
            var parameters = new Dictionary<string, object>();

            string baseQuery = @"
                SELECT Id, CollectionName, Color, NumCards, EstimatedValue, CreationDate, IsComplete 
                FROM Collections";

            if (!string.IsNullOrWhiteSpace(filterField) && !string.IsNullOrWhiteSpace(filterValue) && ValidFields.Contains(filterField))
            {
                whereClause = $" WHERE {filterField} LIKE @FilterValue";
                parameters.Add("@FilterValue", $"%{filterValue}%");
            }

            if (!string.IsNullOrWhiteSpace(sortField) && ValidFields.Contains(sortField))
            {
                var direction = sortDirection?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true ? "DESC" : "ASC";
                orderByClause = $" ORDER BY {sortField} {direction}";
            }

            string finalQuery = baseQuery + whereClause + orderByClause;

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(finalQuery, connection);
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                collections.Add(MapToCollection(reader));
            }

            return collections;
        }

        // POST: AddAsync
        public async Task AddAsync(Collection collection)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = @"
                INSERT INTO Collections 
                (CollectionName, Color, NumCards, EstimatedValue, CreationDate, IsComplete) 
                VALUES (@CollectionName, @Color, @NumCards, @EstimatedValue, @CreationDate, @IsComplete);
                SELECT SCOPE_IDENTITY();"; // devuelve el Id generado

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@CollectionName", collection.CollectionName);
            command.Parameters.AddWithValue("@Color", (object?)collection.Color ?? DBNull.Value);
            command.Parameters.AddWithValue("@NumCards", collection.NumCards);
            command.Parameters.AddWithValue("@EstimatedValue", collection.EstimatedValue);
            command.Parameters.AddWithValue("@CreationDate", collection.CreationDate);
            command.Parameters.AddWithValue("@IsComplete", collection.IsComplete);

            // Obtener el Id generado automáticamente y asignarlo al objeto
            var id = await command.ExecuteScalarAsync();
            collection.Id = Convert.ToInt32(id);
        }

        // PUT: UpdateAsync
        public async Task UpdateAsync(Collection collection)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = @"
                UPDATE Collections SET 
                    CollectionName=@CollectionName, 
                    Color=@Color, 
                    NumCards=@NumCards, 
                    EstimatedValue=@EstimatedValue, 
                    CreationDate=@CreationDate, 
                    IsComplete=@IsComplete
                WHERE Id=@Id";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@CollectionName", collection.CollectionName);
            command.Parameters.AddWithValue("@Color", (object?)collection.Color ?? DBNull.Value);
            command.Parameters.AddWithValue("@NumCards", collection.NumCards);
            command.Parameters.AddWithValue("@EstimatedValue", collection.EstimatedValue);
            command.Parameters.AddWithValue("@CreationDate", collection.CreationDate);
            command.Parameters.AddWithValue("@IsComplete", collection.IsComplete);
            command.Parameters.AddWithValue("@Id", collection.Id);

            await command.ExecuteNonQueryAsync();
        }

        // DELETE
        public async Task DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = "DELETE FROM Collections WHERE Id=@Id";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}