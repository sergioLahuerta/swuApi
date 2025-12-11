using Microsoft.AspNetCore.Mvc;
using swuApi.Models;
using swuApi.PackDTOs;
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

        // GET ALL
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PackGetAllDTO>>> Get(
            [FromQuery] string? filterField,
            [FromQuery] string? filterValue,
            [FromQuery] string? sortField,
            [FromQuery] string? sortDirection)
        {
            var packs = await _packService.GetFilteredAsync(filterField, filterValue, sortField, sortDirection);

            // Mapeo hacia DTOs
            var result = packs.Select(p => new PackGetAllDTO
            {
                Id = p.Id,
                PackName = p.PackName,
                NumberOfCards = p.NumberOfCards,
                ShowcaseRarityOdds = p.ShowcaseRarityOdds,
                GuaranteesRare = p.GuaranteesRare,
                Price = p.Price,
                ReleaseDate = p.ReleaseDate,
                CollectionId = p.CollectionId
            });

            return Ok(result);
        }

        // GET BY ID
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PackGetAllDTO>> Get(int id)
        {
            try
            {
                var pack = await _packService.GetByIdAsync(id);
                if (pack == null)
                    return NotFound();

                var dto = new PackGetAllDTO
                {
                    Id = pack.Id,
                    PackName = pack.PackName,
                    NumberOfCards = pack.NumberOfCards,
                    ShowcaseRarityOdds = pack.ShowcaseRarityOdds,
                    GuaranteesRare = pack.GuaranteesRare,
                    Price = pack.Price,
                    ReleaseDate = pack.ReleaseDate,
                    CollectionId = pack.CollectionId
                };

                return Ok(dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ============================================================
        // POST
        // ============================================================
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] PackCreationDTO dto)
        {
            var pack = new Pack
            {
                PackName = dto.PackName,
                NumberOfCards = dto.NumberOfCards,
                ShowcaseRarityOdds = dto.ShowcaseRarityOdds,
                GuaranteesRare = dto.GuaranteesRare,
                Price = dto.Price,
                ReleaseDate = dto.ReleaseDate,
                CollectionId = dto.CollectionId
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

        // ============================================================
        // PUT
        // ============================================================
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] PackUpdateDTO dto)
        {
            var existing = await _packService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            // Mapeo
            existing.PackName = dto.PackName;
            existing.NumberOfCards = dto.NumberOfCards;
            existing.ShowcaseRarityOdds = dto.ShowcaseRarityOdds;
            existing.GuaranteesRare = dto.GuaranteesRare;
            existing.Price = dto.Price;
            existing.ReleaseDate = dto.ReleaseDate;
            existing.CollectionId = dto.CollectionId;

            try
            {
                await _packService.UpdateAsync(existing);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ============================================================
        // DELETE
        // ============================================================
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _packService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}