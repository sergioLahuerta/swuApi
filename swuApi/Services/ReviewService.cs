using swuApi.Models;
using swuApi.Repositories;

namespace swuApi.Services
{
    public class ReviewService : IService<Review>
    {
        private readonly IRepository<Review> _reviewRepository;

        public ReviewService(IRepository<Review> reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        // GET: por filtrado
        public async Task<List<Review>> GetFilteredAsync(string? filterField, string? filterValue, string? sortField, string? sortDirection)
        {
            return await _reviewRepository.GetFilteredAsync(filterField, filterValue, sortField, sortDirection);
        }

        public async Task<List<Review>> GetAllAsync()
        {
            return await _reviewRepository.GetAllAsync();
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            // Validación de Id, no puede ser 0 o negativo
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.", nameof(id));

            return await _reviewRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Review review)
        {
            // Validación por mensaje, no puede estar vació, tiene que tener algo de texto
            if (string.IsNullOrWhiteSpace(review.MessageReview))
                throw new ArgumentException("El mensaje de la reseña no puede estar vacío.", nameof(review.MessageReview));

            // Validación estrellas de una una reseña
            int reviewValue = (int)review.Stars;
            
            // Validación precio, si es negativo se lanza la excepción
            if (reviewValue < 0 | reviewValue > 5)
            {
                throw new ArgumentException("Las estrellas permitidas en la reseña van de 1 a 5 ambos inclusive.", nameof(reviewValue));
            }
            
            // Validación por fecha, si el usuario no introduce una, por defecto la de ese mismo momento
            if (review.CreationDate == default(DateTime))
            {
                review.CreationDate = DateTime.UtcNow;
            }

            // Validación de fecha, si es un a fecha futura, lanzar excepción
            if (review.CreationDate > DateTime.UtcNow)
                throw new ArgumentException("La fecha de adquisición no puede ser en el futuro.", nameof(review.CreationDate));
            
            await _reviewRepository.AddAsync(review);
        }

        public async Task UpdateAsync(Review review)
        {
            // Validación de Id, no puede ser 0 o negativo
            if (review.Id <= 0)
                throw new ArgumentException("El ID no es válido para actualización.", nameof(review.Id));

            // Validación por mensaje, no puede estar vació, tiene que tener texto
            if (string.IsNullOrWhiteSpace(review.MessageReview))
                throw new ArgumentException("El nombre del review no puede estar vacío.", nameof(review.MessageReview));
            
            // Validación precio, si es negativo se lanza la excepción
            int reviewValue = (int)review.Stars;
            if (reviewValue < 0 | reviewValue > 5)
                throw new ArgumentException("Las estrellas permitidas en la reseña van de 1 a 5 ambos inclusive.", nameof(reviewValue));
            
            // Validación de fecha, si es un a fecha futura, lanzar excepción
            if (review.CreationDate > DateTime.Now)
                throw new ArgumentException("La fecha de adquisición no puede ser en el futuro.", nameof(review.CreationDate));

            await _reviewRepository.UpdateAsync(review);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID no es válido para eliminación.", nameof(id));

            await _reviewRepository.DeleteAsync(id);
        }
    }
}