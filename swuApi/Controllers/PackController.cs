using Microsoft.AspNetCore.Mvc;
using swuApi.Models;
using swuApi.DTOs;
using swuApi.Services;

namespace swuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackController : ControllerBase
    {
        private readonly IService<Pack> _packService;

        public PackController(IService<Pack> packService)
        {
            _packService = packService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Pack>>> Get(
            [FromQuery] string? filterField,
            [FromQuery] string? filterValue,
            [FromQuery] string? sortField,
            [FromQuery] string? sortDirection)
        {
            var packs = await _packService.GetFilteredAsync(filterField, filterValue, sortField, sortDirection);
            return Ok(packs);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Pack>> Get(int id)
        {
            try
            {
                var pack = await _packService.GetByIdAsync(id);
                if (pack == null)
                    return NotFound();

                return Ok(pack);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] PackCreationDTO packDTO)
        {
            var pack = new Pack
            {
                PackName = packDTO.PackName,
                NumberOfCards = packDTO.NumberOfCards,
                ShowcaseRarityOdds = packDTO.ShowcaseRarityOdds,
                GuaranteesRare = packDTO.GuaranteesRare,
                Price = packDTO.Price,
                ReleaseDate = packDTO.ReleaseDate == default ? DateTime.UtcNow : packDTO.ReleaseDate,
                CollectionId = packDTO.CollectionId
            };

            try
            {
                await _packService.AddAsync(pack);

                return CreatedAtAction(nameof(Get), new { id = pack.Id }, pack);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] Pack pack)
        {
            if (id != pack.Id)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del cuerpo.");
            }

            try
            {
                await _packService.UpdateAsync(pack);
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

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _packService.DeleteAsync(id);
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