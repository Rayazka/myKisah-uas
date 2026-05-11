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
//
// 📘 APA INI?
// HTTP endpoint handler untuk Character Companion.
// Route: /api/character
//
// Endpoints:
// - GET /api/character                          → List semua karakter
// - GET /api/character/{id}/response?mood=Happy → Ambil response karakter
//
// 🧠 ENDPOINT KEDUA ADALAH TABLE-DRIVEN ENDPOINT:
// Client mengirim characterId di URL dan mood di query string.
// Controller parse mood ke enum, lalu panggil CharacterService.GenerateResponse().
// Service akan melakukan table lookup tanpa if/switch.
//
// 📋 TODO:
// [ ] 1. Constructor: terima ICharacterService via DI
//
// [ ] 2. Implement GET /api/character — GetAll():
//        → var characters = _service.GetAllCharacters()
//        → return Ok(characters)
//
// [ ] 3. Implement GET /api/character/{id}/response?mood=... — GetResponse(string id, [FromQuery] string mood):
//        a. var parsedMood = Enum.Parse<MoodType>(mood)   (konversi string ke enum)
//        b. var response = _service.GenerateResponse(id, parsedMood)
//        c. return Ok(new { response })                   (return object dengan property 'response')
//
// Tips:
// - [FromQuery] untuk membaca query string ?mood=Happy
// - Enum.Parse<MoodType>(mood) — akan throw exception jika mood tidak valid
//   (ditangkap middleware → 400)
// - Response format JSON: { "response": "teks response..." }
//
// Referensi: Task_myKisah.md baris 211-217

[ApiController]
[Route("api/character")]
public class CharacterController : ControllerBase
{
    private readonly ICharacterService _service;

    public CharacterController(ICharacterService service)
    {
        _service = service;
    }

    // GET /api/character
    [HttpGet]
    public IActionResult GetAll()
    {
        throw new NotImplementedException("TODO: GetAll - panggil service, return Ok");
    }

    // GET /api/character/{id}/response?mood=Happy
    [HttpGet("{id}/response")]
    public IActionResult GetResponse(string id, [FromQuery] string mood)
    {
        throw new NotImplementedException("TODO: GetResponse - parse mood, panggil service.GenerateResponse, return Ok");
    }
}
