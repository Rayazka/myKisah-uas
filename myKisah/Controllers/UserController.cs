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
// PENJELASAN
// HTTP endpoint handler untuk User management.
// Menerima HTTP request, memanggil UserService, mengembalikan HTTP response.

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    // Dependency UserService
    //
    // Menggunakan interface IUserService
    //
    // Tidak implementasi langsung agar loose coupling 
    private readonly IUserService _service;

    // Constructor Dependency Injection
    //
    // ASP.NET otomatis inject UserService saat runtime
    public UserController(IUserService service)
    {
        _service = service;
    }

    // GET /api/user
    [HttpGet]
    public IActionResult GetAll()
    {
        // Ambil semua user dari service, return 200 OK dengan data user
        var users = _service.GetAllUsers();
        return Ok(users);
    }

    // POST /api/user
    [HttpPost]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        // Register user baru, kalau berhasil return 201 Created dengan data user yang baru dibuat
        var user = _service.RegisterUser(request.Username);
        return Ok(user);
    }

    // PUT /api/user/{id}
    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] UpdateUserRequest request)
    {
        // Update user, kalau berhasil return 200 OK dengan data user yang sudah diupdate
        var user = _service.UpdateUser(id, request.Username);
        return Ok(user);
    }

    // DELETE /api/user/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        // Hapus user, kalau berhasil return NoContent (204)
        _service.DeleteUser(id);
        return NoContent();
    }
}

// Request DTO — class sederhana untuk menerima JSON body
public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
}

// DTO untuk update user
public class UpdateUserRequest
{
    public string Username { get; set; } = string.Empty;
}
