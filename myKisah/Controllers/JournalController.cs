using Microsoft.AspNetCore.Mvc;
using myKisah.Interfaces;
using myKisah.Models;

namespace myKisah.Controllers;

// Journal System
// PENANGGUNG JAWAB: Azka

// ** Penjelasan 
// HTTP endpoint handler untuk Journal management.
// Route: /api/journal
// Endpoints:
// - GET  /api/journal/{userId}  → Ambil semua journal milik user
// - POST /api/journal           → Buat journal baru (State=Draft)
// - DELETE /api/journal/{id}    → Hapus journal

[ApiController]
[Route("api/journal")]
public class JournalController : ControllerBase
{
    // Untuk business logic Journal butuh field IJournalService
    private readonly IJournalService _service;

    public JournalController(IJournalService service)
    {
        _service = service;
    }

    // GET /api/journal/{userId}
    [HttpGet("{userId}")]
    public IActionResult GetByUser(string userId)
    {
        // Panggil service untuk dapatkan journal milik user, return Ok dengan data journal
        var journals = _service.GetJournalsByUser(userId);
        return Ok(journals);
    }

    // POST /api/journal
    [HttpPost]
    public IActionResult Create([FromBody] CreateJournalRequest request)
    {
        // Parse mood dari string ke enum untuk dipakai di service
        var mood = Enum.Parse<MoodType>(request.Mood); 

        // panggil service untuk buat journal baru, 
        Journal journal = _service.CreateJournal(request.UserId, request.Title, request.Content, mood);

        // return CreatedAtAction dengan data journal yang baru dibuat
        return CreatedAtAction(nameof(GetByUser), new { userId = journal.UserId }, journal);
        
    }

    // DELETE /api/journal/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        // Panggil service untuk hapus journal berdasarkan Id, 
        // dan return NoContent kalau berhasil dihapus
        _service.DeleteJournal(id);
        return NoContent();
    }
}

// Request DTO untuk membuat journal baru.
// intinya merepresentasikan JSON body dari client ketika buat journal baru (POST /api/journal).
public class CreateJournalRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Mood { get; set; } = string.Empty;
}
