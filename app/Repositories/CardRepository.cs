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

        public async Task<int> GetMaxIdAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = "SELECT ISNULL(MAX(Id), 0) FROM Cards";
            using var command = new SqlCommand(query, connection);

            var result = await command.ExecuteScalarAsync();
            return (int)result;
        }

        public async Task<List<Card>> GetAllAsync()
        {
            var cards = new List<Card>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT Id, CardName, Subtitle, Model, Aspect, CardNumber, Copies, ColectionId FROM Cards";
                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var card = new Card
                        {
                            Id = reader.GetInt32(0),
                            CardName = reader.GetString(1),
                            Subtitle = reader.GetString(2),
                            Model = reader.GetString(3),
                            Aspect = reader.GetString(4),
                            CardNumber = reader.GetInt32(5),
                            Copies = reader.GetInt32(6),
                            ColectionId = reader.GetInt32(7)
                        };
                        cards.Add(card);
                    }
                }
            }
            return cards;
        }

        public async Task<Card> GetByIdAsync(int id)
        {
            Card card = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Id, CardName, Subtitle, Model, Aspect, CardNumber, Copies, ColectionId FROM Cards WHERE Id = @Id";
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
                                Subtitle = reader.GetString(2),
                                Model = reader.GetString(3),
                                Aspect = reader.GetString(4),
                                CardNumber = reader.GetInt32(5),
                                Copies = reader.GetInt32(6),
                                ColectionId = reader.GetInt32(7)
                            };
                        }
                    }
                }
            }
            return card;
        }

        public async Task AddAsync(Card card)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Obtener el max Id actual + 1
            int newId = await GetMaxIdAsync() + 1;
            card.Id = newId;

            string query = @"INSERT INTO Cards (Id, CardName, Subtitle, Model, Aspect, CardNumber, Copies, ColectionId) 
                            VALUES (@Id, @CardName, @Subtitle, @Model, @Aspect, @CardNumber, @Copies, @ColectionId)";
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", card.Id);
            command.Parameters.AddWithValue("@CardName", card.CardName);
            command.Parameters.AddWithValue("@Subtitle", card.Subtitle ?? string.Empty);
            command.Parameters.AddWithValue("@Model", card.Model ?? string.Empty);
            command.Parameters.AddWithValue("@Aspect", card.Aspect ?? string.Empty);
            command.Parameters.AddWithValue("@CardNumber", card.CardNumber);
            command.Parameters.AddWithValue("@Copies", card.Copies);
            command.Parameters.AddWithValue("@ColectionId", card.ColectionId);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Card card)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"UPDATE Cards SET CardName=@CardName, Subtitle=@Subtitle, Model=@Model, Aspect=@Aspect, 
                                 CardNumber=@CardNumber, Copies=@Copies, ColectionId=@ColectionId WHERE Id=@Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", card.Id);
                    command.Parameters.AddWithValue("@CardName", card.CardName);
                    command.Parameters.AddWithValue("@Subtitle", card.Subtitle ?? string.Empty);
                    command.Parameters.AddWithValue("@Model", card.Model ?? string.Empty);
                    command.Parameters.AddWithValue("@Aspect", card.Aspect ?? string.Empty);
                    command.Parameters.AddWithValue("@CardNumber", card.CardNumber);
                    command.Parameters.AddWithValue("@Copies", card.Copies);
                    command.Parameters.AddWithValue("@ColectionId", card.ColectionId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

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

        public IEnumerable<Card> GetAll()
        {
            throw new NotImplementedException();
        }

        public Card GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Add(Card entity)
        {
            throw new NotImplementedException();
        }

        public void Update(Card entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
