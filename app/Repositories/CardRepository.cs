using Microsoft.Data.SqlClient;
using SWUPersonalApi.Models;

namespace SWUPersonalApi.Repositories
{
    public class CartaRepository : IRepository<Carta>
    {
        private readonly string _connectionString;

        public CartaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> GetMaxIdAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = "SELECT ISNULL(MAX(Id), 0) FROM Cartas";
            using var command = new SqlCommand(query, connection);

            var result = await command.ExecuteScalarAsync();
            return (int)result;
        }

        public async Task<List<Carta>> GetAllAsync()
        {
            var cartas = new List<Carta>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT Id, Nombre, Subtitulo, Modelo, Aspecto, NumeroCarta, Cantidad, ColeccionId FROM Cartas";
                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var carta = new Carta
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Subtitulo = reader.GetString(2),
                            Modelo = reader.GetString(3),
                            Aspecto = reader.GetString(4),
                            NumeroCarta = reader.GetInt32(5),
                            Cantidad = reader.GetInt32(6),
                            ColeccionId = reader.GetInt32(7)
                        };
                        cartas.Add(carta);
                    }
                }
            }
            return cartas;
        }

        public async Task<Carta> GetByIdAsync(int id)
        {
            Carta carta = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Id, Nombre, Subtitulo, Modelo, Aspecto, NumeroCarta, Cantidad, ColeccionId FROM Cartas WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            carta = new Carta
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Subtitulo = reader.GetString(2),
                                Modelo = reader.GetString(3),
                                Aspecto = reader.GetString(4),
                                NumeroCarta = reader.GetInt32(5),
                                Cantidad = reader.GetInt32(6),
                                ColeccionId = reader.GetInt32(7)
                            };
                        }
                    }
                }
            }
            return carta;
        }

        public async Task AddAsync(Carta carta)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Obtener el max Id actual + 1
            int newId = await GetMaxIdAsync() + 1;
            carta.Id = newId;

            string query = @"INSERT INTO Cartas (Id, Nombre, Subtitulo, Modelo, Aspecto, NumeroCarta, Cantidad, ColeccionId) 
                            VALUES (@Id, @Nombre, @Subtitulo, @Modelo, @Aspecto, @NumeroCarta, @Cantidad, @ColeccionId)";
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", carta.Id);
            command.Parameters.AddWithValue("@Nombre", carta.Nombre);
            command.Parameters.AddWithValue("@Subtitulo", carta.Subtitulo ?? string.Empty);
            command.Parameters.AddWithValue("@Modelo", carta.Modelo ?? string.Empty);
            command.Parameters.AddWithValue("@Aspecto", carta.Aspecto ?? string.Empty);
            command.Parameters.AddWithValue("@NumeroCarta", carta.NumeroCarta);
            command.Parameters.AddWithValue("@Cantidad", carta.Cantidad);
            command.Parameters.AddWithValue("@ColeccionId", carta.ColeccionId);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Carta carta)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"UPDATE Cartas SET Nombre=@Nombre, Subtitulo=@Subtitulo, Modelo=@Modelo, Aspecto=@Aspecto, 
                                 NumeroCarta=@NumeroCarta, Cantidad=@Cantidad, ColeccionId=@ColeccionId WHERE Id=@Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", carta.Id);
                    command.Parameters.AddWithValue("@Nombre", carta.Nombre);
                    command.Parameters.AddWithValue("@Subtitulo", carta.Subtitulo ?? string.Empty);
                    command.Parameters.AddWithValue("@Modelo", carta.Modelo ?? string.Empty);
                    command.Parameters.AddWithValue("@Aspecto", carta.Aspecto ?? string.Empty);
                    command.Parameters.AddWithValue("@NumeroCarta", carta.NumeroCarta);
                    command.Parameters.AddWithValue("@Cantidad", carta.Cantidad);
                    command.Parameters.AddWithValue("@ColeccionId", carta.ColeccionId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM Cartas WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public IEnumerable<Carta> GetAll()
        {
            throw new NotImplementedException();
        }

        public Carta GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Add(Carta entity)
        {
            throw new NotImplementedException();
        }

        public void Update(Carta entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
