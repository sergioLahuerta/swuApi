using Microsoft.AspNetCore.Mvc;
using swuApi.Services;

public class PackOpeningController : ControllerBase
{
    private readonly IPackOpeningService _packOpeningService;

    // Inyección de dependencias
    public PackOpeningController(IPackOpeningService packOpeningService)
    {
        _packOpeningService = packOpeningService;
    }


    [HttpPost("open/{packId}")]
    public async Task<IActionResult> OpenPack(int packId)
    {
        try
        {
            var generatedCards = await _packOpeningService.OpenPackAsync(packId);

            return Ok(generatedCards);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}