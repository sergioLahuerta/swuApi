using Microsoft.Data.SqlClient;
using swuApi.Models;
using System.Data.Common;

namespace swuApi.Repositories
{
    public class PackRepository : IRepository<Pack>
    {
        private readonly string _connectionString;

        public PackRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private static readonly HashSet<string> ValidFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "PackName", "NumberOfCards", "ShowcaseRarityOdds", "GuaranteesRare", "Price", "ReleaseDate", "CollectionId"
        };

        private Pack MapToPack(DbDataReader reader)
        {
            return new Pack
            {
                Id = reader.GetInt32(0),
                PackName = reader.GetString(1),
                NumberOfCards = reader.GetInt32(2),
                ShowcaseRarityOdds = reader.GetInt32(3),
                GuaranteesRare = reader.GetBoolean(4),
                Price = reader.GetDecimal(5),
                ReleaseDate = reader.GetDateTime(6),
                CollectionId = reader.GetInt32(7)
            };
        }

        
        // Obtener todos los sobres
        public async Task<List<Pack>> GetAllAsync()
        {
            var packs = new List<Pack>();
            
            string query = @"
                SELECT Id, PackName, NumberOfCards, ShowcaseRarityOdds, GuaranteesRare, Price, ReleaseDate, CollectionId 
                FROM Packs";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        packs.Add(MapToPack(reader));
                    }
                }
            }
            return packs;
        }

        // Obtener sobre por ID
        public async Task<Pack?> GetByIdAsync(int id)
        {
            Pack? pack = null;
            string query = @"
                SELECT Id, PackName, NumberOfCards, ShowcaseRarityOdds, GuaranteesRare, Price, ReleaseDate, CollectionId 
                FROM Packs 
                WHERE Id = @Id";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        pack = MapToPack(reader);
                    }
                }
            }
            return pack;
        }

        // Obtener sobres filtrados y ordenados
        public async Task<List<Pack>> GetFilteredAsync(string? filterField, string? filterValue, string? sortField, string? sortDirection)
        {
            var packs = new List<Pack>();
            
            var baseQuery = @"
                SELECT Id, PackName, NumberOfCards, ShowcaseRarityOdds, GuaranteesRare, Price, ReleaseDate, CollectionId 
                FROM Packs";
            
            var whereClause = "";
            var orderByClause = "";
            var parameters = new Dictionary<string, object>();
            
            // Lógica de filtrado
            if (!string.IsNullOrWhiteSpace(filterField) && !string.IsNullOrWhiteSpace(filterValue) && ValidFields.Contains(filterField))
            {
                // Uso LIKE para búsquedas parciales
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
                            packs.Add(MapToPack(reader));
                        }
                    }
                }
            }
            return packs;
        }

        public async Task<int> GetMaxIdAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            string query = "SELECT ISNULL(MAX(Id), 0) FROM Packs";
            using var command = new SqlCommand(query, connection);
            var result = await command.ExecuteScalarAsync();
            return (int)result;
        }
        
        // Añadir nuevo sobre
        public async Task AddAsync(Pack pack)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = @"
                INSERT INTO Packs (PackName, NumberOfCards, ShowcaseRarityOdds, GuaranteesRare, Price, ReleaseDate, CollectionId)
                OUTPUT INSERTED.Id
                VALUES (@PackName, @NumberOfCards, @ShowcaseRarityOdds, @GuaranteesRare, @Price, @ReleaseDate, @CollectionId)";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@PackName", pack.PackName);
            command.Parameters.AddWithValue("@NumberOfCards", pack.NumberOfCards);
            command.Parameters.AddWithValue("@ShowcaseRarityOdds", pack.ShowcaseRarityOdds);
            command.Parameters.AddWithValue("@GuaranteesRare", pack.GuaranteesRare);
            command.Parameters.AddWithValue("@Price", pack.Price);
            command.Parameters.AddWithValue("@ReleaseDate", pack.ReleaseDate);
            command.Parameters.AddWithValue("@CollectionId", pack.CollectionId);

            pack.Id = (int)await command.ExecuteScalarAsync();
        }

        // Actualizar sobre existente
        public async Task UpdateAsync(Pack pack)
        {
            string query = @"
                UPDATE Packs SET 
                    PackName=@PackName, 
                    NumberOfCards=@NumberOfCards, 
                    ShowcaseRarityOdds=@ShowcaseRarityOdds, 
                    GuaranteesRare=@GuaranteesRare, 
                    Price=@Price, 
                    ReleaseDate=@ReleaseDate, 
                    CollectionId=@CollectionId 
                WHERE Id=@Id";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", pack.Id);
                command.Parameters.AddWithValue("@PackName", pack.PackName);
                command.Parameters.AddWithValue("@NumberOfCards", pack.NumberOfCards);
                command.Parameters.AddWithValue("@ShowcaseRarityOdds", pack.ShowcaseRarityOdds);
                command.Parameters.AddWithValue("@GuaranteesRare", pack.GuaranteesRare);
                command.Parameters.AddWithValue("@Price", pack.Price);
                command.Parameters.AddWithValue("@ReleaseDate", pack.ReleaseDate);
                command.Parameters.AddWithValue("@CollectionId", pack.CollectionId);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        // Eliminar sobre
        public async Task DeleteAsync(int id)
        {
            string query = "DELETE FROM Packs WHERE Id = @Id";
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