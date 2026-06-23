// DOMAIN: Character
// Untuk REST API endpoint karakter — GET list karakter, GET response by mood

using Microsoft.AspNetCore.Mvc;
using myKisah.Interfaces;
using myKisah.Models;

namespace myKisah.Controllers;

// ═══════════════════════════════════════════════════════════
// KELAS: CharacterController
// DOMAIN: Character Companion System
// TEKNIK: API Development
// PENANGGUNG JAWAB: Toni Kurniawan
// ═══════════════════════════════════════════════════════════


[ApiController]
[Route("api/character")]
public class CharacterController : ControllerBase
{
    private readonly ICharacterService _service;

    public CharacterController(ICharacterService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var characters = _service.GetAllCharacters();
        return Ok(characters);
    }

    [HttpGet("{id}/response")]
    public IActionResult GetResponse(string id, [FromQuery] string mood)
    {
        var parsedMood = Enum.Parse<MoodType>(mood);
        var response = _service.GenerateResponse(id, parsedMood);
        return Ok(new { response });
    }
}
