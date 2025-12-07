using Microsoft.Data.SqlClient;
using swuApi.Models;
using swuApi.Enums;
using System.Data.Common;

namespace swuApi.Repositories
{
    public class CardRepository : IPackOpeningRepository
    {
        private readonly string _connectionString;

        public CardRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private static readonly HashSet<string> ValidFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "CardName", "Model", "Aspect", "Rarity", "CardNumber", "Copies", "CollectionId", "Price", "DateAcquired", "IsPromo"
        };

        // Función de mapeo unificada para manejar la conversión de string a enum
        private Card MapToCard(DbDataReader reader)
        {
            string aspectString = reader.IsDBNull(4) ? "None" : reader.GetString(4);
            string subtitleString = reader.IsDBNull(2) ? null : reader.GetString(2);

            return new Card
            {
                Id = reader.GetInt32(0),
                CardName = reader.GetString(1),
                Subtitle = subtitleString,

                // Mapeo del enum, obteniendo la cadena del .sql y conviertiéndola a enum
                Model = (CardModelType)Enum.Parse(typeof(CardModelType), reader.GetString(3)),
                Aspect = (CardAspectType)Enum.Parse(typeof(CardAspectType), aspectString),
                Rarity = (CardRarityType)Enum.Parse(typeof(CardRarityType), reader.GetString(5)),
                CardNumber = reader.GetInt32(6),
                Copies = reader.GetInt32(7),
                CollectionId = reader.GetInt32(8),
                Price = reader.GetDecimal(9),
                DateAcquired = reader.GetDateTime(10),
                IsPromo = reader.GetBoolean(11)
            };
        }

        // Método para obtener el ID máximo
        public async Task<int> GetMaxIdAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = "SELECT ISNULL(MAX(Id), 0) FROM Cards";
            using var command = new SqlCommand(query, connection);

            var result = await command.ExecuteScalarAsync();
            return (int)result;
        }

        // GetFilteredAsync: Obtener Cartas filtradas y ordenadas
        public async Task<List<Card>> GetFilteredAsync(string? filterField, string? filterValue, string? sortField, string? sortDirection)
        {
            var cards = new List<Card>();

            // 💡 Consulta actualizada para incluir Rarity (índice 5)
            var baseQuery = @"
                SELECT Id, CardName, Subtitle, Model, Aspect, Rarity, CardNumber, Copies, CollectionId, Price, DateAcquired, IsPromo FROM Cards";

            var whereClause = "";
            var orderByClause = "";
            var parameters = new Dictionary<string, object>();

            // Cláusula where para filtraje
            if (!string.IsNullOrWhiteSpace(filterField) && !string.IsNullOrWhiteSpace(filterValue))
            {
                // Valido que el campo existe
                if (ValidFields.Contains(filterField))
                {
                    // LIKE para búsquedas parciales
                    whereClause = $" WHERE {filterField} LIKE @FilterValue";
                    parameters.Add("@FilterValue", $"%{filterValue}%");
                }
            }

            // También construir la cláusula ORDER BY
            if (!string.IsNullOrWhiteSpace(sortField))
            {
                // Lo mismo
                if (ValidFields.Contains(sortField))
                {
                    // Determinar la dirección ascendente
                    var direction = "ASC";
                    if (sortDirection != null && sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                    {
                        direction = "DESC";
                    }

                    // Inserción del campo validado y la dirección
                    orderByClause = $" ORDER BY {sortField} {direction}";
                }
            }

            // Añadir a la consulta el filtrado
            string finalQuery = baseQuery + whereClause + orderByClause;

            // Ejecutarla
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(finalQuery, connection))
                {
                    // Añadir parámetros de filtrado si existen
                    foreach (var param in parameters)
                    {
                        // Uso AddWithValue para la seguridad contra inyección SQL
                        command.Parameters.AddWithValue(param.Key, param.Value);
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            cards.Add(MapToCard(reader));
                        }
                    }
                }
            }
            return cards;
        }

        // GetAllAsync: Obtener todas las Cartas
        public async Task<List<Card>> GetAllAsync()
        {
            var cards = new List<Card>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT Id, CardName, Subtitle, Model, Aspect, Rarity, CardNumber, Copies, CollectionId, Price, DateAcquired, IsPromo FROM Cards";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        cards.Add(MapToCard(reader));
                    }
                }
            }
            return cards;
        }

        // GetByIdAsync: Obtener Carta por ID
        public async Task<Card?> GetByIdAsync(int id)
        {
            Card? card = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT Id, CardName, Subtitle, Model, Aspect, Rarity, CardNumber, Copies, CollectionId, Price, DateAcquired, IsPromo FROM Cards WHERE Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            card = MapToCard(reader);
                        }
                    }
                }
            }
            return card;
        }

        public async Task<List<Card>> GetAllCardsInCollectionAsync(int collectionId)
        {
            // Lógica para obtener las cartas filtrando por CollectionId
            string query = "SELECT Id, CardName, Subtitle, Model, Aspect, Rarity, CardNumber, Copies, CollectionId, Price, DateAcquired, IsPromo FROM Cards WHERE CollectionId = @CollectionId";

            var cards = new List<Card>();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CollectionId", collectionId);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        cards.Add(MapToCard(reader));
                    }
                }
            }
            return cards;
        }

        // AddAsync: Crear nueva Carta
        public async Task AddAsync(Card card)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            int newId = await GetMaxIdAsync() + 1;
            card.Id = newId;

            string query = @"INSERT INTO Cards (Id, CardName, Subtitle, Model, Aspect, Rarity, CardNumber, Copies, CollectionId, Price, DateAcquired, IsPromo) 
                             VALUES (@Id, @CardName, @Subtitle, @Model, @Aspect, @Rarity, @CardNumber, @Copies, @CollectionId, @Price, @DateAcquired, @IsPromo)";

            using var command = new SqlCommand(query, connection);

            // Uso .ToString() para convertir el enum a la cadena NVARCHAR del .sql
            command.Parameters.AddWithValue("@Id", card.Id);
            command.Parameters.AddWithValue("@CardName", card.CardName);
            command.Parameters.AddWithValue("@Subtitle", card.Subtitle ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Model", card.Model.ToString());
            command.Parameters.AddWithValue("@Aspect", card.Aspect.ToString());
            command.Parameters.AddWithValue("@Rarity", card.Rarity.ToString());
            command.Parameters.AddWithValue("@CardNumber", card.CardNumber);
            command.Parameters.AddWithValue("@Copies", card.Copies);
            command.Parameters.AddWithValue("@CollectionId", card.CollectionId);
            command.Parameters.AddWithValue("@Price", card.Price);
            command.Parameters.AddWithValue("@DateAcquired", card.DateAcquired);
            command.Parameters.AddWithValue("@IsPromo", card.IsPromo);

            await command.ExecuteNonQueryAsync();
        }

        // UpdateAsync: Actualizar Carta existente
        public async Task UpdateAsync(Card card)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"UPDATE Cards SET CardName=@CardName, Subtitle=@Subtitle, Model=@Model, Aspect=@Aspect, Rarity=@Rarity, CardNumber=@CardNumber, Copies=@Copies, CollectionId=@CollectionId, Price=@Price, DateAcquired=@DateAcquired, IsPromo=@IsPromo WHERE Id=@Id";

                using (var command = new SqlCommand(query, connection))
                {
                    // Uso .ToString() para convertir el enum a la cadena NVARCHAR del .sql
                    command.Parameters.AddWithValue("@Id", card.Id);
                    command.Parameters.AddWithValue("@CardName", card.CardName);
                    command.Parameters.AddWithValue("@Subtitle", card.Subtitle ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Model", card.Model.ToString());
                    command.Parameters.AddWithValue("@Aspect", card.Aspect.ToString());
                    command.Parameters.AddWithValue("@Rarity", card.Rarity.ToString());
                    command.Parameters.AddWithValue("@CardNumber", card.CardNumber);
                    command.Parameters.AddWithValue("@Copies", card.Copies);
                    command.Parameters.AddWithValue("@CollectionId", card.CollectionId);
                    command.Parameters.AddWithValue("@Price", card.Price);
                    command.Parameters.AddWithValue("@DateAcquired", card.DateAcquired);
                    command.Parameters.AddWithValue("@IsPromo", card.IsPromo);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // DeleteAsync: Eliminar Carta
        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM Cards WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}