using Microsoft.Data.SqlClient;
using swuApi.Models;
using System.Collections.Generic;
using System.Data;

namespace swuApi.Repositories
{
    public class CollectionRepository : IRepository<Collection>
    {
        private readonly string _connectionString;
        
        // Campos para filtrar y ordenar Colecciones
        private static readonly HashSet<string> ValidFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "CollectionName", "Color", "NumCards", "EstimatedValue", "CreationDate", "IsComplete"
        };

        public CollectionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Helper: Obtener el siguiente ID para inserción manual
        public async Task<int> GetMaxIdAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = "SELECT ISNULL(MAX(Id), 0) FROM Collections";
            using var command = new SqlCommand(query, connection);

            var result = await command.ExecuteScalarAsync();
            return (int)result;
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
        
        // GET: Implementación de filtraje y ordenación
        public async Task<List<Collection>> GetFilteredAsync(string? filterField, string? filterValue, string? sortField, string? sortDirection)
        {
            var collections = new List<Collection>();
            var whereClause = "";
            var orderByClause = "";
            var parameters = new Dictionary<string, object>();

            // Definiciñon de la consulta base
            var baseQuery = @"
                SELECT Id, CollectionName, Color, NumCards, EstimatedValue, CreationDate, IsComplete 
                FROM Collections";

            // Filtraje
            if (!string.IsNullOrWhiteSpace(filterField) && !string.IsNullOrWhiteSpace(filterValue) && ValidFields.Contains(filterField))
            {
                whereClause = $" WHERE {filterField} LIKE @FilterValue";
                parameters.Add("@FilterValue", $"%{filterValue}%");
            }

            // Ordenación
            if (!string.IsNullOrWhiteSpace(sortField) && ValidFields.Contains(sortField))
            {
                var direction = (sortDirection != null && sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)) ? "DESC" : "ASC";
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
                            collections.Add(MapToCollection(reader));
                        }
                    }
                }
            }
            return collections;
        }


        // GET: GetAllAsync
        public async Task<List<Collection>> GetAllAsync()
        {
            // Reutilizo la lógica del mapeo
            var collections = new List<Collection>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"
                    SELECT Id, CollectionName, Color, NumCards, EstimatedValue, CreationDate, IsComplete 
                    FROM Collections";
                
                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        collections.Add(MapToCollection(reader));
                    }
                }
            }
            return collections;
        }

        // GET: GetByIdAsync
        public async Task<Collection?> GetByIdAsync(int id)
        {
            Collection? collection = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"
                    SELECT Id, CollectionName, Color, NumCards, EstimatedValue, CreationDate, IsComplete 
                    FROM Collections WHERE Id = @Id";
                
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            collection = MapToCollection(reader);
                        }
                    }
                }
            }
            return collection;
        }

        // POST: AddAsync
        public async Task AddAsync(Collection collection)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            int newId = await GetMaxIdAsync() + 1;
            collection.Id = newId;

            string query = @"INSERT INTO Collections (Id, CollectionName, Color, NumCards, EstimatedValue, CreationDate, IsComplete) 
                             VALUES (@Id, @CollectionName, @Color, @NumCards, @EstimatedValue, @CreationDate, @IsComplete)";
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", collection.Id);
            command.Parameters.AddWithValue("@CollectionName", collection.CollectionName);
            command.Parameters.AddWithValue("@Color", collection.Color ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@NumCards", collection.NumCards);
            command.Parameters.AddWithValue("@EstimatedValue", collection.EstimatedValue);
            command.Parameters.AddWithValue("@CreationDate", collection.CreationDate);
            command.Parameters.AddWithValue("@IsComplete", collection.IsComplete);

            await command.ExecuteNonQueryAsync();
        }

        // PUT: UpdateAsync
        public async Task UpdateAsync(Collection collection)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"UPDATE Collections SET CollectionName=@CollectionName, Color=@Color, NumCards=@NumCards, 
                                 EstimatedValue=@EstimatedValue, CreationDate=@CreationDate, IsComplete=@IsComplete
                                 WHERE Id=@Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", collection.Id);
                    command.Parameters.AddWithValue("@CollectionName", collection.CollectionName);
                    command.Parameters.AddWithValue("@Color", collection.Color ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NumCards", collection.NumCards);
                    command.Parameters.AddWithValue("@EstimatedValue", collection.EstimatedValue);
                    command.Parameters.AddWithValue("@CreationDate", collection.CreationDate);
                    command.Parameters.AddWithValue("@IsComplete", collection.IsComplete);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // DELETE: DeleteAsync
        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM Collections WHERE Id = @Id"; 
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}