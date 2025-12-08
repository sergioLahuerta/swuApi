using Microsoft.AspNetCore.Mvc;
using swuApi.Models;
using swuApi.UserCardDTOs;
using swuApi.Services;
using swuApi.DTOs;

namespace swuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserCardController : ControllerBase
    {
        private readonly IService<UserCard> _userCardService;

        public UserCardController(IService<UserCard> userCardService)
        {
            _userCardService = userCardService;
        }

        // GET: api/UserCard
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserCardGetAllDTO>>> Get()
        {
            var userCards = await _userCardService.GetAllAsync();

            // Mapeo a DTO
            var dtoList = userCards.Select(uc => new UserCardGetAllDTO
            {
                Id = uc.Id,
                UserId = uc.UserId,
                CardId = uc.CardId,
                Copies = uc.Copies,
                DateAdded = uc.DateAdded,
                IsFavorite = uc.IsFavorite,
                UserName = uc.User?.Username,
                CardName = uc.Card?.CardName
            });

            return Ok(dtoList);
        }

        // GET: api/UserCard/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserCardGetAllDTO>> Get(int id)
        {
            var userCard = await _userCardService.GetByIdAsync(id);
            if (userCard == null) return NotFound();

            var dto = new UserCardGetAllDTO
            {
                Id = userCard.Id,
                UserId = userCard.UserId,
                CardId = userCard.CardId,
                Copies = userCard.Copies,
                DateAdded = userCard.DateAdded,
                IsFavorite = userCard.IsFavorite,
                UserName = userCard.User?.Username,
                CardName = userCard.Card?.CardName
            };

            return Ok(dto);
        }

        // POST: api/UserCard
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserCardCreationDTO dto)
        {
            var userCard = new UserCard
            {
                UserId = dto.UserId,
                CardId = dto.CardId,
                Copies = dto.Copies,
                DateAdded = DateTime.UtcNow,
                IsFavorite = dto.IsFavorite
            };

            await _userCardService.AddAsync(userCard);

            return CreatedAtAction(nameof(Get), new { id = userCard.Id }, userCard);
        }

        // PUT: api/UserCard/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserCardUpdateDTO dto)
        {
            var existing = await _userCardService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.UserId = dto.UserId;
            existing.CardId = dto.CardId;
            existing.Copies = dto.Copies;
            existing.DateAdded = dto.DateAdded ?? existing.DateAdded;
            existing.IsFavorite = dto.IsFavorite;

            await _userCardService.UpdateAsync(existing);

            return NoContent();
        }

        // DELETE: api/UserCard/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _userCardService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _userCardService.DeleteAsync(id);
            return NoContent();
        }
    }
}