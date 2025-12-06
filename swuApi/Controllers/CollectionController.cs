using Microsoft.AspNetCore.Mvc;
using swuApi.Models;
using swuApi.Services;

namespace swuApi.Controllers
{
    [ApiController]
    // La ruta será /api/Colections
    [Route("api/[controller]")] 
    public class ColectionController : ControllerBase
    {
        // Inyectamos el Servicio, que contendrá la lógica de negocio y usará el Repositorio.
        private readonly IService<Colection> _colectionService;

        public ColectionController(IService<Colection> colectionService)
        {
            _colectionService = colectionService;
        }

        // GET: api/Colections
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Colection>>> Get()
        {
            var colections = await _colectionService.GetAllAsync();
            return Ok(colections);
        }

        // GET: api/Colections/5 (uno específico)
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Colection>> Get(int id)
        {
            try
            {
                var colection = await _colectionService.GetByIdAsync(id);
                if (colection == null)
                    return NotFound();

                return Ok(colection);
            }
            // Capturar la excepción si el ID es inválido
            catch (ArgumentException ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Colections
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] Colection colection)
        {
            try
            {
                await _colectionService.AddAsync(colection); 

                // Código de estado 201 Created, con la URL del nuevo recurso
                return CreatedAtAction(nameof(Get), new { id = colection.Id }, colection); 
            }
            // Capturo excepción de validación como que el nombre esté vacíoo
            catch (ArgumentException ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Colections/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] Colection colection)
        {
            if (id != colection.Id)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del cuerpo.");
            }

            try
            {
                await _colectionService.UpdateAsync(colection);
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
        // DELETE: api/Colections/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _colectionService.DeleteAsync(id);
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