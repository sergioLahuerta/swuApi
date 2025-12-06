using Microsoft.Data.SqlClient;
using swuApi.Models;

namespace swuApi.Repositories
{
    public class CardRepository : IRepository<Card>
    {
        private readonly string _connectionString;

        public CardRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private static readonly HashSet<string> ValidFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "CardName", "Model", "Aspect", "CardNumber", "Copies", "CollectionId", "Price", "DateAcquired", "IsPromo"
        };

        public async Task<List<Card>> GetFilteredAsync(string? filterField, string? filterValue, string? sortField, string? sortDirection)
        {
            var cards = new List<Card>();
            
            var baseQuery = @"
                SELECT Id, CardName, Subtitle, Model, Aspect, CardNumber, Copies, CollectionId, 
                       Price, DateAcquired, IsPromo 
                FROM Cards";
            
            var whereClause = "";
            var orderByClause = "";
            var parameters = new Dictionary<string, object>();
            
            // 2. Construir la cláusula WHERE (Filtrado)
            if (!string.IsNullOrWhiteSpace(filterField) && !string.IsNullOrWhiteSpace(filterValue))
            {
                // Seguridad: Validar que el campo existe
                if (ValidFields.Contains(filterField))
                {
                    // Usamos LIKE para búsquedas parciales
                    whereClause = $" WHERE {filterField} LIKE @FilterValue";
                    parameters.Add("@FilterValue", $"%{filterValue}%");
                }
            }

            // 3. Construir la cláusula ORDER BY (Ordenación)
            if (!string.IsNullOrWhiteSpace(sortField))
            {
                // Seguridad: Validar que el campo existe
                if (ValidFields.Contains(sortField))
                {
                    // Determinar la dirección (por defecto ASC)
                    var direction = "ASC";
                    if (sortDirection != null && sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                    {
                        direction = "DESC";
                    }
                    
                    // Nota: Se inserta el campo validado y la dirección
                    orderByClause = $" ORDER BY {sortField} {direction}";
                }
            }

            // 4. Combinar la consulta
            string finalQuery = baseQuery + whereClause + orderByClause;

            // 5. Ejecutar la consulta
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(finalQuery, connection))
                {
                    // Añadir parámetros de filtrado si existen
                    foreach (var param in parameters)
                    {
                        // Usamos AddWithValue para la seguridad contra inyección SQL
                        command.Parameters.AddWithValue(param.Key, param.Value);
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Mapeo de la Carta (Idéntico al GetAllAsync)
                            var card = new Card
                            {
                                Id = reader.GetInt32(0),
                                CardName = reader.GetString(1),
                                Subtitle = reader.IsDBNull(2) ? null : reader.GetString(2), 
                                Model = reader.GetString(3),
                                Aspect = reader.IsDBNull(4) ? null : reader.GetString(4),
                                CardNumber = reader.GetInt32(5),
                                Copies = reader.GetInt32(6),
                                CollectionId = reader.GetInt32(7),
                                Price = reader.GetDecimal(8), 
                                DateAcquired = reader.GetDateTime(9),
                                IsPromo = reader.GetBoolean(10)
                            };
                            cards.Add(card);
                        }
                    }
                }
            }
            return cards;
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

        // GetAllAsync: Obtener todas las Cartas
        public async Task<List<Card>> GetAllAsync()
        {
            var cards = new List<Card>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT Id, CardName, Subtitle, Model, Aspect, CardNumber, Copies, CollectionId,
                           Price, DateAcquired, IsPromo 
                    FROM Cards";
                
                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var card = new Card
                        {
                            Id = reader.GetInt32(0),
                            CardName = reader.GetString(1),
                            Subtitle = reader.IsDBNull(2) ? null : reader.GetString(2), 
                            Model = reader.GetString(3),
                            Aspect = reader.IsDBNull(4) ? null : reader.GetString(4),
                            CardNumber = reader.GetInt32(5),
                            Copies = reader.GetInt32(6),
                            CollectionId = reader.GetInt32(7),
                            Price = reader.GetDecimal(8), 
                            DateAcquired = reader.GetDateTime(9),
                            IsPromo = reader.GetBoolean(10)
                        };
                        cards.Add(card);
                    }
                }
            }
            return cards;
        }

        // GetByIdAsync: Obtener Carta por ID
        public async Task<Card> GetByIdAsync(int id)
        {
            Card card = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                string query = @"
                    SELECT Id, CardName, Subtitle, Model, Aspect, CardNumber, Copies, CollectionId, 
                           Price, DateAcquired, IsPromo 
                    FROM Cards WHERE Id = @Id";
                
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            card = new Card
                            {
                                Id = reader.GetInt32(0),
                                CardName = reader.GetString(1),
                                Subtitle = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Model = reader.GetString(3),
                                Aspect = reader.IsDBNull(4) ? null : reader.GetString(4),
                                CardNumber = reader.GetInt32(5),
                                Copies = reader.GetInt32(6),
                                CollectionId = reader.GetInt32(7),
                                Price = reader.GetDecimal(8),
                                DateAcquired = reader.GetDateTime(9),
                                IsPromo = reader.GetBoolean(10)
                            };
                        }
                    }
                }
            }
            return card;
        }

        // AddAsync: Crear nueva Carta
        public async Task AddAsync(Card card)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            int newId = await GetMaxIdAsync() + 1;
            card.Id = newId;

            string query = @"INSERT INTO Cards (Id, CardName, Subtitle, Model, Aspect, CardNumber, Copies, CollectionId, Price, DateAcquired, IsPromo) 
                             VALUES (@Id, @CardName, @Subtitle, @Model, @Aspect, @CardNumber, @Copies, @CollectionId, @Price, @DateAcquired, @IsPromo)";
            
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", card.Id);
            command.Parameters.AddWithValue("@CardName", card.CardName);
            command.Parameters.AddWithValue("@Subtitle", card.Subtitle ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Model", card.Model);
            command.Parameters.AddWithValue("@Aspect", card.Aspect ?? (object)DBNull.Value);
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
                
                string query = @"UPDATE Cards SET CardName=@CardName, Subtitle=@Subtitle, Model=@Model, Aspect=@Aspect, 
                                 CardNumber=@CardNumber, Copies=@Copies, CollectionId=@CollectionId, 
                                 Price=@Price, DateAcquired=@DateAcquired, IsPromo=@IsPromo WHERE Id=@Id";
                
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", card.Id);
                    command.Parameters.AddWithValue("@CardName", card.CardName);
                    command.Parameters.AddWithValue("@Subtitle", card.Subtitle ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Model", card.Model);
                    command.Parameters.AddWithValue("@Aspect", card.Aspect ?? (object)DBNull.Value);
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