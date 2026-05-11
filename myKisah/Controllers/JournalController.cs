using Microsoft.AspNetCore.Mvc;
using myKisah.Interfaces;
using myKisah.Models;

namespace myKisah.Controllers;

// ═══════════════════════════════════════════════════════════
// KELAS: JournalController
// DOMAIN: Journal System
// TEKNIK: API Development
// PENANGGUNG JAWAB: Rayazka Aris
// ═══════════════════════════════════════════════════════════
//
// 📘 APA INI?
// HTTP endpoint handler untuk Journal management.
// Route: /api/journal
//
// Endpoints:
// - GET  /api/journal/{userId}  → Ambil semua journal milik user
// - POST /api/journal           → Buat journal baru (State=Draft)
// - DELETE /api/journal/{id}    → Hapus journal
//
// 📋 TODO:
// [ ] 1. Constructor: terima IJournalService via DI
//
// [ ] 2. Implement GET /api/journal/{userId} — GetByUser(string userId):
//        → var journals = _service.GetJournalsByUser(userId)
//        → return Ok(journals)
//
// [ ] 3. Implement POST /api/journal — Create([FromBody] CreateJournalRequest request):
//        → var journal = _service.CreateJournal(
//              request.UserId, request.Title, request.Content,
//              Enum.Parse<MoodType>(request.Mood))
//        → return CreatedAtAction(nameof(GetById), new { id = journal.Id }, journal)
//        (atau return Ok(journal))
//
// [ ] 4. Implement DELETE /api/journal/{id} — Delete(string id):
//        → _service.DeleteJournal(id)
//        → return NoContent()
//
// Tips:
// - Mood dikirim sebagai string oleh client → parse ke enum di controller
// - Pakai Enum.Parse<MoodType>(request.Mood) untuk konversi
// - Exception dari invalid parse akan ditangkap middleware (ArgumentException → 400)
//
// Referensi: Task_myKisah.md baris 211-217

[ApiController]
[Route("api/journal")]
public class JournalController : ControllerBase
{
    private readonly IJournalService _service;

    public JournalController(IJournalService service)
    {
        _service = service;
    }

    // GET /api/journal/{userId}
    [HttpGet("{userId}")]
    public IActionResult GetByUser(string userId)
    {
        throw new NotImplementedException("TODO: GetByUser - panggil service, return Ok");
    }

    // POST /api/journal
    [HttpPost]
    public IActionResult Create([FromBody] CreateJournalRequest request)
    {
        throw new NotImplementedException("TODO: Create - parse mood, panggil service, return Ok/Created");
    }

    // DELETE /api/journal/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        throw new NotImplementedException("TODO: Delete - panggil service, return NoContent");
    }
}

// Request DTO
public class CreateJournalRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Mood { get; set; } = string.Empty;
}
