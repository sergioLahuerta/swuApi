namespace swuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartaController : ControllerBase
    {
        private readonly IRepository<Carta> _repository;

        public CartaController(IRepository<Carta> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Carta>>> Get()
        {
            var cartas = await _repository.GetAllAsync();
            return Ok(cartas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Carta>> Get(int id)
        {
            var carta = await _repository.GetByIdAsync(id);
            if (carta == null)
                return NotFound();
            return Ok(carta);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Carta carta)
        {
            await _repository.AddAsync(carta);
            return CreatedAtAction(nameof(Get), new { id = carta.Id }, carta);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Carta carta)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();
            await _repository.UpdateAsync(carta);
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
