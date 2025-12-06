namespace swuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardController : ControllerBase
    {
        private readonly IRepository<Card> _repository;

        public CardController(IRepository<Card> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Card>>> Get()
        {
            var cards = await _repository.GetAllAsync();
            return Ok(cards);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Card>> Get(int id)
        {
            var card = await _repository.GetByIdAsync(id);
            if (card == null)
                return NotFound();
            return Ok(card);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Card card)
        {
            await _repository.AddAsync(card);
            return CreatedAtAction(nameof(Get), new { id = card.Id }, card);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Card card)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();
            await _repository.UpdateAsync(card);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
