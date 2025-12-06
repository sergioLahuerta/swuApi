using Microsoft.AspNetCore.Mvc;
using swuApi.Models;
using swuApi.DTOs   ;
using swuApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace swuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardController : ControllerBase
    {
        private readonly IService<Card> _cardService;

        public CardController(IService<Card> cardService)
        {
            _cardService = cardService;
        }

        // GET: api/Card?filterField=Aspect&filterValue=Vigilance&sortField=Price&sortDirection=desc
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Card>>> Get(
            [FromQuery] string? filterField,
            [FromQuery] string? filterValue,
            [FromQuery] string? sortField,
            [FromQuery] string? sortDirection)
        {
            var cards = await _cardService.GetFilteredAsync(filterField, filterValue, sortField, sortDirection);
            return Ok(cards);
        }

        // GET: api/Card/3
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Card>> Get(int id)
        {
            try
            {
                var card = await _cardService.GetByIdAsync(id);
                if (card == null)
                    return NotFound();
                return Ok(card);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Card
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CardCreationDTO cardDTO)
        {
            // Mapeo DTO a Modelo Card
            var card = new Card
            {
                CardName = cardDTO.CardName,
                Subtitle = cardDTO.Subtitle,
                Model = cardDTO.Model,
                Aspect = cardDTO.Aspect,
                CardNumber = cardDTO.CardNumber,
                Copies = cardDTO.Copies,
                Price = cardDTO.Price,
                DateAcquired = cardDTO.DateAcquired,
                IsPromo = cardDTO.IsPromo,
                CollectionId = cardDTO.CollectionId
            };

            try
            {
                await _cardService.AddAsync(card);

                return CreatedAtAction(nameof(Get), new { id = card.Id }, card);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Card/3
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] Card card)
        {
            if (id != card.Id)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del cuerpo.");
            }

            try
            {
                await _cardService.UpdateAsync(card);
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

        // DELETE: api/Card/3
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Asumimos que el servicio verifica la existencia antes de eliminar
                await _cardService.DeleteAsync(id);
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