using Microsoft.AspNetCore.Mvc;
using myKisah.Interfaces;

namespace myKisah.Controllers;

// ═══════════════════════════════════════════════════════════
// KELAS: UserController
// DOMAIN: User Management
// TEKNIK: API Development
// PENANGGUNG JAWAB: Farel Ilham
// ═══════════════════════════════════════════════════════════
//
// 📘 APA INI?
// HTTP endpoint handler untuk User management.
// Menerima HTTP request, memanggil UserService, mengembalikan HTTP response.
//
// Route: /api/user
//
// 🧠 KENAPA CONTROLLER TERPISAH DARI SERVICE?
// Controller = urusan HTTP (parsing request, return status code).
// Service = urusan bisnis (validasi, logic).
// Pemisahan ini membuat keduanya bisa di-test secara terpisah.
//
// Semua exception dari Service ditangkap oleh ErrorHandlingMiddleware.
// Controller TIDAK perlu try-catch.
//
// 📋 TODO:
// [ ] 1. Constructor: terima IUserService via DI
//
// [ ] 2. Implement GET /api/user — GetAll():
//        → var users = _service.GetAllUsers()
//        → return Ok(users)
//
// [ ] 3. Implement POST /api/user — Register([FromBody] RegisterRequest request):
//        → var user = _service.RegisterUser(request.Username)
//        → return CreatedAtAction(nameof(GetById), new { id = user.Id }, user)
//        (atau return Ok(user) kalau tidak ada GetById)
//
// [ ] 4. Implement PUT /api/user/{id} — Update(string id, [FromBody] UpdateRequest request):
//        → var user = _service.UpdateUser(id, request.Username)
//        → return Ok(user)
//
// [ ] 5. Implement DELETE /api/user/{id} — Delete(string id):
//        → _service.DeleteUser(id)
//        → return NoContent()
//
// Tips:
// - Pakai [ApiController] attribute → automatic model validation
// - [FromBody] untuk membaca JSON request body
// - [FromRoute] atau langsung string id di parameter (ASP.NET otomatis bind)
// - Request DTO ada di bawah (RegisterRequest, UpdateRequest)
//
// Referensi: Task_myKisah.md baris 211-217

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    // GET /api/user
    [HttpGet]
    public IActionResult GetAll()
    {
        throw new NotImplementedException("TODO: GetAll - panggil service, return Ok");
    }

    // POST /api/user
    [HttpPost]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        throw new NotImplementedException("TODO: Register - panggil service, return Created или Ok");
    }

    // PUT /api/user/{id}
    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] UpdateUserRequest request)
    {
        throw new NotImplementedException("TODO: Update - panggil service, return Ok");
    }

    // DELETE /api/user/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        throw new NotImplementedException("TODO: Delete - panggil service, return NoContent");
    }
}

// Request DTO — class sederhana untuk menerima JSON body
public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
}

public class UpdateUserRequest
{
    public string Username { get; set; } = string.Empty;
}
