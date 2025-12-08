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

    // En Controllers/PackOpeningController.cs

    [HttpPost("open/{packId}")] // Ejemplo de ruta: POST /api/PackOpening/open/1
    public async Task<IActionResult> OpenPack(int packId)
    {
        try
        {
            // 🚨 Llamas al método de tu servicio
            var generatedCards = await _packOpeningService.OpenPackAsync(packId);

            // Retornas el resultado al cliente
            return Ok(generatedCards);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}