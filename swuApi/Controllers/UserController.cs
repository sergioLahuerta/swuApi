using Microsoft.AspNetCore.Mvc;
using swuApi.Models;
using swuApi.UserDTOs;
using swuApi.Services;

namespace swuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IService<User> _userService;

        public UserController(IService<User> userService)
        {
            _userService = userService;
        }

        // ============================================================
        // GET ALL USERS (Con filtros + DTO)
        // ============================================================
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserGetAllDTO>>> Get(
            [FromQuery] string? filterField,
            [FromQuery] string? filterValue,
            [FromQuery] string? sortField,
            [FromQuery] string? sortDirection)
        {
            var users = await _userService.GetFilteredAsync(filterField, filterValue, sortField, sortDirection);

            var result = users.Select(u => new UserGetAllDTO
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                PasswordHash = u.PasswordHash,
                RegistrationDate = u.RegistrationDate,
                IsActive = u.IsActive,
                TotalCollectionValue = u.TotalCollectionValue
            });

            return Ok(result);
        }

        // ============================================================
        // GET BY ID
        // ============================================================
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserGetAllDTO>> Get(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                    return NotFound();

                var dto = new UserGetAllDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    PasswordHash = user.PasswordHash,
                    RegistrationDate = user.RegistrationDate,
                    IsActive = user.IsActive,
                    TotalCollectionValue = user.TotalCollectionValue
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
        public async Task<IActionResult> Post([FromBody] UserCreationDTO dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = dto.PasswordHash,
                RegistrationDate = dto.RegistrationDate,
                IsActive = dto.IsActive,
                TotalCollectionValue = dto.TotalCollectionValue
            };

            try
            {
                await _userService.AddAsync(user);

                return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
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
        public async Task<IActionResult> Put(int id, [FromBody] UserUpdateDTO dto)
        {
            var existing = await _userService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            existing.Username = dto.Username;
            existing.Email = dto.Email;
            existing.PasswordHash = dto.PasswordHash;
            existing.RegistrationDate = dto.RegistrationDate;
            existing.IsActive = dto.IsActive;
            existing.TotalCollectionValue = dto.TotalCollectionValue;

            try
            {
                await _userService.UpdateAsync(existing);
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
                await _userService.DeleteAsync(id);
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