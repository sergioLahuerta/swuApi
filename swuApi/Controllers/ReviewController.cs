using Microsoft.AspNetCore.Mvc;
using swuApi.Models;
using swuApi.Services;
using swuApi.ReviewDTOs;

namespace swuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IService<Review> _reviewService;

        public ReviewController(IService<Review> reviewService)
        {
            _reviewService = reviewService;
        }

        // GET: api/Review?filterField=Stars&filterValue=One&sortField=
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Review>>> Get(
            [FromQuery] string? filterField,
            [FromQuery] string? filterValue,
            [FromQuery] string? sortField,
            [FromQuery] string? sortDirection)
        {
            var reviews = await _reviewService.GetFilteredAsync(filterField, filterValue, sortField, sortDirection);

            // Devuelve directamente el modelo Review (sin mapear a DTO)
            return Ok(reviews);
        }

        // GET: api/Review/3
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReviewGetByIdDTO>> Get(int id)
        {
            try
            {
                var review = await _reviewService.GetByIdAsync(id);
                if (review == null)
                    return NotFound();

                var dto = new ReviewGetByIdDTO
                {
                    Id = review.Id,
                    CreationDate = review.CreationDate,
                    MessageReview = review.MessageReview,
                    Stars = review.Stars,
                    UserId = review.UserId
                };

                return Ok(dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Review
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] ReviewCreationDTO reviewDTO)
        {
            // Mapeo DTO a Modelo Review
            var review = new Review
            {
                CreationDate = reviewDTO.CreationDate,
                MessageReview = reviewDTO.MessageReview,
                Stars = reviewDTO.Stars,
                UserId = reviewDTO.UserId
            };

            try
            {
                await _reviewService.AddAsync(review);

                return CreatedAtAction(nameof(Get), new { id = review.Id }, review);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Review/3
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] ReviewUpdateDTO reviewUpdateDTO) 
        {
            // 1. Mapeo del DTO al modelo de dominio (Review)
            var reviewToUpdate = new Review 
            {
                Id = id,
                CreationDate = reviewUpdateDTO.CreationDate,
                MessageReview = reviewUpdateDTO.MessageReview,
                Stars = reviewUpdateDTO.Stars,
                UserId = reviewUpdateDTO.UserId
            };

            // 2. Ejecutar la lógica de servicio
            try
            {
                await _reviewService.UpdateAsync(reviewToUpdate);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE: api/Review/3
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Asumimos que el servicio verifica la existencia antes de eliminar
                await _reviewService.DeleteAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}