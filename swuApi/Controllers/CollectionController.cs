using Microsoft.AspNetCore.Mvc;
using swuApi.Models;
using swuApi.DTOs;
using swuApi.Services;

namespace swuApi.Controllers
{
    [ApiController]
    // La ruta será /api/Collections
    [Route("api/[controller]")] 
    public class CollectionController : ControllerBase
    {
        // Inyectamos el Servicio, que contendrá la lógica de negocio y usará el Repositorio.
        private readonly IService<Collection> _collectionService;

        public CollectionController(IService<Collection> collectionService)
        {
            _collectionService = collectionService;
        }

        // GET: api/Collections
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Collection>>> Get()
        {
            var collections = await _collectionService.GetAllAsync();
            return Ok(collections);
        }

        // GET: api/Collections/5 (uno específico)
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Collection>> Get(int id)
        {
            try
            {
                var collection = await _collectionService.GetByIdAsync(id);
                if (collection == null)
                    return NotFound();

                return Ok(collection);
            }
            // Capturar la excepción si el ID es inválido
            catch (ArgumentException ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Collections
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CollectionCreationDTO collectionDTO)
        {
            // Mapeao del DTO al Modelo Collection
            var collection = new Collection
            {
                CollectionName = collectionDTO.CollectionName,
                Color = collectionDTO.Color,
                NumCards = collectionDTO.NumCards,
                EstimatedValue = collectionDTO.EstimatedValue,
                
                // Usamos el valor del DTO, si es nulo, el servicio pondrá UtcNow
                CreationDate = collectionDTO.CreationDate ?? default(DateTime), 
                
                IsComplete = collectionDTO.IsComplete,
                
                // Las propiedades de navegación (Cards) se omiten en la creación
            };

            try
            {
                await _collectionService.AddAsync(collection);

                return CreatedAtAction(nameof(Get), new { id = collection.Id }, collection);
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Collections/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] Collection collection)
        {
            if (id != collection.Id)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del cuerpo.");
            }

            try
            {
                await _collectionService.UpdateAsync(collection);
                return NoContent(); // Código de estado 204 No Content (Éxito sin contenido de respuesta)
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // 400
            }
            catch (KeyNotFoundException) // Asumiendo que el servicio lanza esto si no existe
            {
                return NotFound(); // 404
            }
        }

        // ------------------------------------
        // DELETE: api/Collections/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _collectionService.DeleteAsync(id);
                return NoContent(); // No content es éxito porque una petición DELETE no devuelve nada (en el cuerpo)
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