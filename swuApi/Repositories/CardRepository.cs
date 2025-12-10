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
            "CardName", "Model", "Aspect", "Rarity", "CardNumber", "CollectionId", "Price", "DateAcquired", "IsPromo"
        };

        // Funci├│n de mapeo unificada para manejar la conversi├│n de string a enum (Se mantiene igual)
        private Card MapToCard(DbDataReader reader)
        {
            string aspectString = reader.IsDBNull(4) ? "None" : reader.GetString(4);
            string subtitleString = reader.IsDBNull(2) ? null : reader.GetString(2);

            return new Card
            {
                Id = reader.GetInt32(0),
                CardName = reader.GetString(1),
                Subtitle = subtitleString,
                Model = (CardModelType)Enum.Parse(typeof(CardModelType), reader.GetString(3)),
                Aspect = (CardAspectType)Enum.Parse(typeof(CardAspectType), aspectString),
                Rarity = (CardRarityType)Enum.Parse(typeof(CardRarityType), reader.GetString(5)),
                CardNumber = reader.GetInt32(6),
                CollectionId = reader.GetInt32(7),
                Price = reader.GetDecimal(8),
                DateAcquired = reader.GetDateTime(9),
                IsPromo = reader.GetBoolean(10)
            };
        }

        // GetFilteredAsync (Se mantiene igual)
        public async Task<List<Card>> GetFilteredAsync(string? filterField, string? filterValue, string? sortField, string? sortDirection)
        {
            var cards = new List<Card>();

            var baseQuery = @"
                SELECT 
                    c.Id, c.CardName, c.Subtitle, c.Model, c.Aspect, c.Rarity, c.CardNumber, c.CollectionId, 
                    c.Price, c.DateAcquired, c.IsPromo,
                    col.Id AS ColId, col.CollectionName, col.Color, col.NumCards, col.EstimatedValue, col.CreationDate, col.IsComplete
                FROM Cards c
                LEFT JOIN Collections col ON c.CollectionId = col.Id";

            var whereClause = "";
            var orderByClause = "";
            var parameters = new Dictionary<string, object>();

            // Filtro dinámico
            if (!string.IsNullOrWhiteSpace(filterField) && !string.IsNullOrWhiteSpace(filterValue))
            {
                if (ValidFields.Contains(filterField))
                {
                    whereClause = $" WHERE c.{filterField} LIKE @FilterValue";
                    parameters.Add("@FilterValue", $"%{filterValue}%");
                }
            }

            // Orden dinámico
            if (!string.IsNullOrWhiteSpace(sortField))
            {
                if (ValidFields.Contains(sortField))
                {
                    var direction = "ASC";
                    if (!string.IsNullOrWhiteSpace(sortDirection) && sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                    {
                        direction = "DESC";
                    }
                    orderByClause = $" ORDER BY c.{sortField} {direction}";
                }
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
                            // Mapeo de Card con Collection
                            var card = new Card
                            {
                                Id = reader.GetInt32(0),
                                CardName = reader.GetString(1),
                                Subtitle = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Model = (CardModelType)Enum.Parse(typeof(CardModelType), reader.GetString(3)),
                                Aspect = (CardAspectType)Enum.Parse(typeof(CardAspectType), reader.GetString(4)),
                                Rarity = (CardRarityType)Enum.Parse(typeof(CardRarityType), reader.GetString(5)),
                                CardNumber = reader.GetInt32(6),
                                CollectionId = reader.GetInt32(7),
                                Price = reader.GetDecimal(8),
                                DateAcquired = reader.GetDateTime(9),
                                IsPromo = reader.GetBoolean(10),
                                Collection = reader.IsDBNull(11) ? null : new Collection
                                {
                                    Id = reader.GetInt32(11),
                                    CollectionName = reader.GetString(12),
                                    Color = reader.GetString(13),
                                    NumCards = reader.GetInt32(14),
                                    EstimatedValue = reader.GetDecimal(15),
                                    CreationDate = reader.GetDateTime(16),
                                    IsComplete = reader.GetBoolean(17)
                                }
                            };

                            cards.Add(card);
                        }
                    }
                }
            }

            return cards;
        }

        // GetAllAsync
        public async Task<List<Card>> GetAllAsync()
        {
            var cards = new List<Card>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT Id, CardName, Subtitle, Model, Aspect, Rarity, CardNumber, CollectionId, Price, DateAcquired, IsPromo FROM Cards";

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

        // GetByIdAsync (Se mantiene igual)
        public async Task<Card?> GetByIdAsync(int id)
        {
            Card? card = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT Id, CardName, Subtitle, Model, Aspect, Rarity, CardNumber, CollectionId, Price, DateAcquired, IsPromo FROM Cards WHERE Id = @Id";

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

        // GetAllCardsInCollectionAsync (Se mantiene igual)
        public async Task<List<Card>> GetAllCardsInCollectionAsync(int collectionId)
        {
            string query = "SELECT Id, CardName, Subtitle, Model, Aspect, Rarity, CardNumber, CollectionId, Price, DateAcquired, IsPromo FROM Cards WHERE CollectionId = @CollectionId";

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

        // AddAsync: POST
        public async Task AddAsync(Card card)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = @"INSERT INTO Cards
                             (CardName, Subtitle, Model, Aspect, Rarity, CardNumber, CollectionId, Price, DateAcquired, IsPromo)
                             VALUES (@CardName, @Subtitle, @Model, @Aspect, @Rarity, @CardNumber, @CollectionId, @Price, @DateAcquired, @IsPromo);
                             SELECT SCOPE_IDENTITY();";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@CardName", card.CardName);
            command.Parameters.AddWithValue("@Subtitle", card.Subtitle ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Model", card.Model.ToString());
            command.Parameters.AddWithValue("@Aspect", card.Aspect.ToString());
            command.Parameters.AddWithValue("@Rarity", card.Rarity.ToString());
            command.Parameters.AddWithValue("@CardNumber", card.CardNumber);
            command.Parameters.AddWithValue("@CollectionId", card.CollectionId);
            command.Parameters.AddWithValue("@Price", card.Price);
            command.Parameters.AddWithValue("@DateAcquired", card.DateAcquired);
            command.Parameters.AddWithValue("@IsPromo", card.IsPromo);

            // 5. Ejecutar como escalar y asignar el ID de vuelta
            var newId = await command.ExecuteScalarAsync();

            if (newId != null && newId != DBNull.Value)
            {
                card.Id = Convert.ToInt32(newId);
            }
        }

        // UpdateAsync: PUT
        public async Task UpdateAsync(Card card)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"UPDATE Cards SET CardName=@CardName, Subtitle=@Subtitle, Model=@Model, Aspect=@Aspect, Rarity=@Rarity, CardNumber=@CardNumber, CollectionId=@CollectionId, Price=@Price, DateAcquired=@DateAcquired, IsPromo=@IsPromo WHERE Id=@Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", card.Id);
                    command.Parameters.AddWithValue("@CardName", card.CardName);
                    command.Parameters.AddWithValue("@Subtitle", card.Subtitle ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Model", card.Model.ToString());
                    command.Parameters.AddWithValue("@Aspect", card.Aspect.ToString());
                    command.Parameters.AddWithValue("@Rarity", card.Rarity.ToString());
                    command.Parameters.AddWithValue("@CardNumber", card.CardNumber);
                    command.Parameters.AddWithValue("@CollectionId", card.CollectionId);
                    command.Parameters.AddWithValue("@Price", card.Price);
                    command.Parameters.AddWithValue("@DateAcquired", card.DateAcquired);
                    command.Parameters.AddWithValue("@IsPromo", card.IsPromo);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // DeleteAsync (Se mantiene igual)
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

        public Task<List<Card>> GetAllReviewsInUserAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}