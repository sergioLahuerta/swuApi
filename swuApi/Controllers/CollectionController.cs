using Microsoft.AspNetCore.Mvc;
using swuApi.Models;
using swuApi.Services;
using swuApi.CollectionDTOs;

namespace swuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CollectionController : ControllerBase
    {
        private readonly IService<Collection> _collectionService;

        public CollectionController(IService<Collection> collectionService)
        {
            _collectionService = collectionService;
        }

        // ----------------------------------------
        // GET ALL
        // ----------------------------------------
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CollectionGetAllDTO>>> Get()
        {
            var collections = await _collectionService.GetAllAsync();

            var dtoList = collections.Select(c => new CollectionGetAllDTO
            {
                Id = c.Id,
                CollectionName = c.CollectionName,
                Color = c.Color,
                NumCards = c.NumCards,
                EstimatedValue = c.EstimatedValue,
                CreationDate = c.CreationDate,
                IsComplete = c.IsComplete
            });

            return Ok(dtoList);
        }

        // ----------------------------------------
        // GET BY ID
        // ----------------------------------------
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CollectionGetAllDTO>> Get(int id)
        {
            var collection = await _collectionService.GetByIdAsync(id);

            if (collection == null)
                return NotFound();

            var dto = new CollectionGetAllDTO
            {
                Id = collection.Id,
                CollectionName = collection.CollectionName,
                Color = collection.Color,
                NumCards = collection.NumCards,
                EstimatedValue = collection.EstimatedValue,
                CreationDate = collection.CreationDate,
                IsComplete = collection.IsComplete
            };

            return Ok(dto);
        }

        // ----------------------------------------
        // POST
        // ----------------------------------------
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CollectionCreationDTO dto)
        {
            var collection = new Collection
            {
                CollectionName = dto.CollectionName,
                Color = dto.Color,
                NumCards = dto.NumCards,
                EstimatedValue = dto.EstimatedValue,
                CreationDate = dto.CreationDate ?? DateTime.Now,
                IsComplete = dto.IsComplete
            };

            await _collectionService.AddAsync(collection);

            return CreatedAtAction(nameof(Get), new { id = collection.Id }, collection);
        }

        // ----------------------------------------
        // PUT
        // ----------------------------------------
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] CollectionUpdateDTO dto)
        {
            var existing = await _collectionService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            existing.CollectionName = dto.CollectionName;
            existing.Color = dto.Color;
            existing.NumCards = dto.NumCards;
            existing.EstimatedValue = dto.EstimatedValue;
            existing.CreationDate = dto.CreationDate ?? existing.CreationDate;
            existing.IsComplete = dto.IsComplete;

            await _collectionService.UpdateAsync(existing);

            return NoContent();
        }

        // ----------------------------------------
        // DELETE
        // ----------------------------------------
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var collection = await _collectionService.GetByIdAsync(id);
            if (collection == null)
                return NotFound();

            await _collectionService.DeleteAsync(id);
            return NoContent();
        }
    }
}